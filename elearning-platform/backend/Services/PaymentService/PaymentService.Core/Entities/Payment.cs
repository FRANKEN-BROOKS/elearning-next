using System;
using System.Collections.Generic;
using Shared.Common.Entities;

namespace PaymentService.Core.Entities
{
    /// <summary>
    /// Payment entity
    /// </summary>
    public class Payment : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid OrderId { get; set; }
        public string TransactionId { get; set; } // Omise charge ID
        public string PaymentMethod { get; set; } // CreditCard, DebitCard, PromptPay, TrueMoney
        public string PaymentProvider { get; set; } = "Omise";
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "THB";
        public string Status { get; set; } = "Pending"; // Pending, Processing, Successful, Failed, Cancelled, Refunded
        public string Description { get; set; }
        public string FailureReason { get; set; }
        public DateTime? PaymentDate { get; set; }

        public virtual Order Order { get; set; }
    }

    /// <summary>
    /// Order entity
    /// </summary>
    public class Order : BaseEntity
    {
        public Guid UserId { get; set; }
        public string OrderNumber { get; set; }
        public string OrderType { get; set; } // CourseEnrollment, Subscription
        public Guid ReferenceId { get; set; } // CourseId or SubscriptionPlanId
        public decimal SubtotalThb { get; set; }
        public decimal DiscountThb { get; set; }
        public decimal TaxThb { get; set; }
        public decimal TotalThb { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled, Refunded
        public string CouponCode { get; set; }
        public string Notes { get; set; }

        public virtual ICollection<OrderItem> Items { get; set; }
    }

    /// <summary>
    /// Order item entity
    /// </summary>
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; set; }
        public string ItemType { get; set; } // Course, Subscription
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal UnitPriceThb { get; set; }
        public decimal DiscountThb { get; set; }
        public decimal TotalPriceThb { get; set; }

        public virtual Order Order { get; set; }
    }

    /// <summary>
    /// Payment method entity
    /// </summary>
    public class PaymentMethod : BaseEntity
    {
        public Guid UserId { get; set; }
        public string MethodType { get; set; } // CreditCard, DebitCard
        public string CardBrand { get; set; }
        public string LastFourDigits { get; set; }
        public int? ExpiryMonth { get; set; }
        public int? ExpiryYear { get; set; }
        public string CardHolderName { get; set; }
        public string OmiseCardId { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Refund entity
    /// </summary>
    public class Refund : BaseEntity
    {
        public Guid PaymentId { get; set; }
        public decimal RefundAmount { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Completed
        public string OmiseRefundId { get; set; }
        public Guid? ProcessedBy { get; set; }
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public virtual Payment Payment { get; set; }
    }

    /// <summary>
    /// Payment webhook entity
    /// </summary>
    public class PaymentWebhook : BaseEntity
    {
        public string EventId { get; set; }
        public string EventType { get; set; }
        public Guid? PaymentId { get; set; }
        public string PayloadJson { get; set; }
        public bool IsProcessed { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string ErrorMessage { get; set; }
        public int RetryCount { get; set; }
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

        public virtual Payment Payment { get; set; }
    }

    /// <summary>
    /// Payment transaction log entity
    /// </summary>
    public class PaymentTransactionLog : BaseEntity
    {
        public Guid PaymentId { get; set; }
        public string Action { get; set; } // CreateCharge, CaptureCharge, RefundCharge
        public string RequestData { get; set; }
        public string ResponseData { get; set; }
        public int? StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }

        public virtual Payment Payment { get; set; }
    }

    /// <summary>
    /// Invoice entity
    /// </summary>
    public class Invoice : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Guid? PaymentId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime IssueDate { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public string Status { get; set; } = "Draft"; // Draft, Sent, Paid, Overdue, Cancelled
        public decimal SubtotalThb { get; set; }
        public decimal TaxThb { get; set; }
        public decimal TotalThb { get; set; }
        public string PdfUrl { get; set; }
        public string SentToEmail { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public string Notes { get; set; }

        public virtual Order Order { get; set; }
        public virtual Payment Payment { get; set; }
    }
}