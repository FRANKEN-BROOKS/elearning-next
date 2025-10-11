using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Common.DTOs;
using Shared.MessageQueue;
using Shared.MessageQueue.Events;
using PaymentService.Core.Entities;
using PaymentService.Infrastructure.Data;
using PaymentService.Infrastructure.Services;

namespace PaymentService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentDbContext _context;
        private readonly IOmiseService _omiseService;
        private readonly IMessageQueueService _messageQueue;

        public PaymentsController(
            PaymentDbContext context,
            IOmiseService omiseService,
            IMessageQueueService messageQueue)
        {
            _context = context;
            _omiseService = omiseService;
            _messageQueue = messageQueue;
        }

        [HttpPost("create-charge")]
        public async Task<ActionResult<ApiResponse<Payment>>> CreateCharge([FromBody] CreateChargeRequest request)
        {
            try
            {
                // Create order first
                var order = new Order
                {
                    UserId = request.UserId,
                    OrderNumber = GenerateOrderNumber(),
                    OrderType = "CourseEnrollment",
                    ReferenceId = request.CourseId,
                    SubtotalThb = request.Amount,
                    TotalThb = request.Amount,
                    Status = "Pending"
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Create payment record
                var payment = new Payment
                {
                    UserId = request.UserId,
                    OrderId = order.Id,
                    PaymentMethod = request.PaymentMethod,
                    Amount = request.Amount,
                    Currency = "THB",
                    Status = "Pending",
                    Description = request.Description
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                // Create charge with Omise
                var chargeRequest = new OmiseChargeRequest
                {
                    AmountThb = request.Amount,
                    Description = request.Description,
                    CardToken = request.CardToken,
                    ReturnUri = request.ReturnUri,
                    OrderId = order.Id,
                    UserId = request.UserId
                };

                var chargeResponse = await _omiseService.CreateChargeAsync(chargeRequest);

                // Update payment with transaction details
                payment.TransactionId = chargeResponse.Id;
                payment.Status = MapOmiseStatusToPaymentStatus(chargeResponse.Status);
                
                if (chargeResponse.Status == "successful")
                {
                    payment.PaymentDate = DateTime.UtcNow;
                    order.Status = "Confirmed";
                    
                    // Publish payment received event
                    await _messageQueue.PublishAsync("payment.received", new PaymentReceivedEvent
                    {
                        PaymentId = payment.Id,
                        OrderId = order.Id,
                        UserId = payment.UserId,
                        Amount = payment.Amount,
                        Currency = payment.Currency,
                        Status = payment.Status,
                        TransactionId = payment.TransactionId
                    });
                }
                else if (chargeResponse.Status == "failed")
                {
                    payment.FailureReason = chargeResponse.FailureMessage;
                    order.Status = "Cancelled";
                }

                await _context.SaveChangesAsync();

                // Log transaction
                var log = new PaymentTransactionLog
                {
                    PaymentId = payment.Id,
                    Action = "CreateCharge",
                    IsSuccess = chargeResponse.Status == "successful",
                    ResponseData = System.Text.Json.JsonSerializer.Serialize(chargeResponse)
                };
                _context.PaymentTransactionLogs.Add(log);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<Payment>.SuccessResponse(payment, "Payment processed"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<Payment>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Payment>>> GetById(Guid id)
        {
            try
            {
                var payment = await _context.Payments
                    .Include(p => p.Order)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (payment == null)
                {
                    return NotFound(ApiResponse<Payment>.ErrorResponse("Payment not found", 404));
                }

                return Ok(ApiResponse<Payment>.SuccessResponse(payment));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<Payment>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponse<List<Payment>>>> GetUserPayments(Guid userId)
        {
            try
            {
                var payments = await _context.Payments
                    .Include(p => p.Order)
                    .Where(p => p.UserId == userId)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                return Ok(ApiResponse<List<Payment>>.SuccessResponse(payments));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<Payment>>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook()
        {
            try
            {
                using var reader = new System.IO.StreamReader(Request.Body);
                var payload = await reader.ReadToEndAsync();

                // Store webhook for processing
                var webhook = new PaymentWebhook
                {
                    EventId = Guid.NewGuid().ToString(),
                    EventType = "omise.charge",
                    PayloadJson = payload,
                    IsProcessed = false
                };

                _context.PaymentWebhooks.Add(webhook);
                await _context.SaveChangesAsync();

                // Process webhook asynchronously
                // In production, use a background job processor
                _ = ProcessWebhookAsync(webhook.Id);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{id}/refund")]
        public async Task<ActionResult<ApiResponse<Refund>>> CreateRefund(Guid id, [FromBody] CreateRefundRequest request)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null)
                {
                    return NotFound(ApiResponse<Refund>.ErrorResponse("Payment not found", 404));
                }

                if (payment.Status != "Successful")
                {
                    return BadRequest(ApiResponse<Refund>.ErrorResponse("Can only refund successful payments"));
                }

                // Create refund in Omise
                var refundResponse = await _omiseService.CreateRefundAsync(
                    payment.TransactionId, 
                    request.Amount
                );

                // Create refund record
                var refund = new Refund
                {
                    PaymentId = payment.Id,
                    RefundAmount = request.Amount,
                    Reason = request.Reason,
                    Status = "Completed",
                    OmiseRefundId = refundResponse.Id,
                    ProcessedAt = DateTime.UtcNow,
                    CompletedAt = DateTime.UtcNow
                };

                _context.Refunds.Add(refund);

                // Update payment status
                payment.Status = "Refunded";
                
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<Refund>.SuccessResponse(refund, "Refund processed successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<Refund>.ErrorResponse(ex.Message));
            }
        }

        private string GenerateOrderNumber()
        {
            return $"ORD{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }

        private string MapOmiseStatusToPaymentStatus(string omiseStatus)
        {
            return omiseStatus switch
            {
                "successful" => "Successful",
                "pending" => "Pending",
                "failed" => "Failed",
                _ => "Pending"
            };
        }

        private async Task ProcessWebhookAsync(Guid webhookId)
        {
            // Background webhook processing logic
            await Task.Delay(100);
        }
    }

    // Request DTOs
    public class CreateChargeRequest
    {
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string CardToken { get; set; }
        public string Description { get; set; }
        public string ReturnUri { get; set; }
    }

    public class CreateRefundRequest
    {
        public decimal Amount { get; set; }
        public string Reason { get; set; }
    }
}