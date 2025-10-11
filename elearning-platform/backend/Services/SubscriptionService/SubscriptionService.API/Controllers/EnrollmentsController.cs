using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shared.Common.DTOs;
using Shared.MessageQueue;
using Shared.MessageQueue.Events;
using SubscriptionService.Core.Entities;
using SubscriptionService.Infrastructure.Repositories;

namespace SubscriptionService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IMessageQueueService _messageQueue;

        public EnrollmentsController(
            IEnrollmentRepository enrollmentRepository,
            ICouponRepository couponRepository,
            IMessageQueueService messageQueue)
        {
            _enrollmentRepository = enrollmentRepository;
            _couponRepository = couponRepository;
            _messageQueue = messageQueue;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponse<List<Enrollment>>>> GetUserEnrollments(Guid userId)
        {
            try
            {
                var enrollments = await _enrollmentRepository.GetByUserIdAsync(userId);
                return Ok(ApiResponse<List<Enrollment>>.SuccessResponse(enrollments));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<Enrollment>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Enrollment>>> GetById(Guid id)
        {
            try
            {
                var enrollment = await _enrollmentRepository.GetByIdAsync(id);
                if (enrollment == null)
                {
                    return NotFound(ApiResponse<Enrollment>.ErrorResponse("Enrollment not found", 404));
                }
                return Ok(ApiResponse<Enrollment>.SuccessResponse(enrollment));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<Enrollment>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Enrollment>>> CreateEnrollment([FromBody] CreateEnrollmentRequest request)
        {
            try
            {
                // Check if already enrolled
                var existing = await _enrollmentRepository.GetByUserAndCourseAsync(request.UserId, request.CourseId);
                if (existing != null)
                {
                    return BadRequest(ApiResponse<Enrollment>.ErrorResponse("Already enrolled in this course"));
                }

                decimal finalPrice = request.PriceThb;

                // Apply coupon if provided
                if (!string.IsNullOrEmpty(request.CouponCode))
                {
                    var isValid = await _couponRepository.ValidateCouponAsync(
                        request.CouponCode, 
                        request.UserId, 
                        request.PriceThb
                    );

                    if (!isValid)
                    {
                        return BadRequest(ApiResponse<Enrollment>.ErrorResponse("Invalid or expired coupon"));
                    }

                    var coupon = await _couponRepository.GetByCodeAsync(request.CouponCode);
                    if (coupon != null)
                    {
                        if (coupon.DiscountType == "Percentage")
                        {
                            finalPrice = request.PriceThb * (1 - coupon.DiscountValue / 100);
                        }
                        else if (coupon.DiscountType == "FixedAmount")
                        {
                            finalPrice = Math.Max(0, request.PriceThb - coupon.DiscountValue);
                        }

                        if (coupon.MaximumDiscount.HasValue)
                        {
                            var discount = request.PriceThb - finalPrice;
                            if (discount > coupon.MaximumDiscount.Value)
                            {
                                finalPrice = request.PriceThb - coupon.MaximumDiscount.Value;
                            }
                        }
                    }
                }

                var enrollment = new Enrollment
                {
                    UserId = request.UserId,
                    CourseId = request.CourseId,
                    PriceThb = finalPrice,
                    PaymentStatus = "Pending",
                    Status = "Active"
                };

                var created = await _enrollmentRepository.CreateAsync(enrollment);

                // Publish enrollment event
                await _messageQueue.PublishAsync("course.enrolled", new CourseEnrolledEvent
                {
                    EnrollmentId = created.Id,
                    UserId = created.UserId,
                    CourseId = created.CourseId,
                    PriceThb = created.PriceThb
                });

                return Ok(ApiResponse<Enrollment>.SuccessResponse(created, "Enrollment created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<Enrollment>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<ApiResponse<bool>>> CancelEnrollment(Guid id, [FromBody] CancelEnrollmentRequest request)
        {
            try
            {
                var enrollment = await _enrollmentRepository.GetByIdAsync(id);
                if (enrollment == null)
                {
                    return NotFound(ApiResponse<bool>.ErrorResponse("Enrollment not found", 404));
                }

                enrollment.Status = "Cancelled";
                enrollment.CancelledAt = DateTime.UtcNow;
                enrollment.CancelledBy = request.CancelledBy;
                enrollment.CancellationReason = request.Reason;

                await _enrollmentRepository.UpdateAsync(enrollment);

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Enrollment cancelled successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id}/complete")]
        public async Task<ActionResult<ApiResponse<Enrollment>>> CompleteEnrollment(Guid id)
        {
            try
            {
                var enrollment = await _enrollmentRepository.GetByIdAsync(id);
                if (enrollment == null)
                {
                    return NotFound(ApiResponse<Enrollment>.ErrorResponse("Enrollment not found", 404));
                }

                enrollment.IsCompleted = true;
                enrollment.CompletedAt = DateTime.UtcNow;
                enrollment.CompletionPercentage = 100;

                await _enrollmentRepository.UpdateAsync(enrollment);

                // Publish course completed event for certificate generation
                await _messageQueue.PublishAsync("course.completed", new CourseCompletedEvent
                {
                    UserId = enrollment.UserId,
                    CourseId = enrollment.CourseId
                });

                return Ok(ApiResponse<Enrollment>.SuccessResponse(enrollment, "Course completed"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<Enrollment>.ErrorResponse(ex.Message));
            }
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistRepository _wishlistRepository;

        public WishlistController(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponse<List<Wishlist>>>> GetUserWishlist(Guid userId)
        {
            try
            {
                var wishlist = await _wishlistRepository.GetByUserIdAsync(userId);
                return Ok(ApiResponse<List<Wishlist>>.SuccessResponse(wishlist));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<Wishlist>>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Wishlist>>> Add([FromBody] AddToWishlistRequest request)
        {
            try
            {
                var existing = await _wishlistRepository.GetByUserAndCourseAsync(request.UserId, request.CourseId);
                if (existing != null)
                {
                    return BadRequest(ApiResponse<Wishlist>.ErrorResponse("Course already in wishlist"));
                }

                var wishlist = new Wishlist
                {
                    UserId = request.UserId,
                    CourseId = request.CourseId,
                    Notes = request.Notes
                };

                var added = await _wishlistRepository.AddAsync(wishlist);
                return Ok(ApiResponse<Wishlist>.SuccessResponse(added, "Added to wishlist"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<Wishlist>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete]
        public async Task<ActionResult<ApiResponse<bool>>> Remove([FromQuery] Guid userId, [FromQuery] Guid courseId)
        {
            try
            {
                await _wishlistRepository.RemoveAsync(userId, courseId);
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Removed from wishlist"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }
    }

    // DTOs
    public class CreateEnrollmentRequest
    {
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public decimal PriceThb { get; set; }
        public string CouponCode { get; set; }
    }

    public class CancelEnrollmentRequest
    {
        public Guid CancelledBy { get; set; }
        public string Reason { get; set; }
    }

    public class AddToWishlistRequest
    {
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public string Notes { get; set; }
    }
}