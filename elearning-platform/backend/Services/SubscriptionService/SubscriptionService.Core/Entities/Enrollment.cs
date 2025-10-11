using System;
using Shared.Common.Entities;

namespace SubscriptionService.Core.Entities
{
    /// <summary>
    /// Enrollment entity
    /// </summary>
    public class Enrollment : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiryDate { get; set; }
        public string Status { get; set; } = "Active"; // Active, Expired, Cancelled, Suspended
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Completed, Failed, Refunded
        public decimal PriceThb { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public DateTime? LastAccessedAt { get; set; }
        public decimal CompletionPercentage { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public Guid? CancelledBy { get; set; }
        public string CancellationReason { get; set; }
    }

    /// <summary>
    /// Enrollment history entity
    /// </summary>
    public class EnrollmentHistory : BaseEntity
    {
        public Guid EnrollmentId { get; set; }
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }
        public Guid? ChangedBy { get; set; }
        public string Reason { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        public virtual Enrollment Enrollment { get; set; }
    }

    /// <summary>
    /// Wishlist entity
    /// </summary>
    public class Wishlist : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public string Notes { get; set; }
    }

    /// <summary>
    /// Course notification entity
    /// </summary>
    public class CourseNotification : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public string NotificationType { get; set; } // NewLesson, PriceChange, CourseUpdate
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
    }

    /// <summary>
    /// Coupon entity
    /// </summary>
    public class Coupon : BaseEntity
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string DiscountType { get; set; } // Percentage, FixedAmount
        public decimal DiscountValue { get; set; }
        public decimal? MinimumPurchase { get; set; }
        public decimal? MaximumDiscount { get; set; }
        public int? UsageLimit { get; set; }
        public int UsageCount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string ApplicableCourseIds { get; set; } // JSON array
        public bool IsActive { get; set; } = true;
        public Guid CreatedBy { get; set; }
    }

    /// <summary>
    /// User coupon usage entity
    /// </summary>
    public class UserCoupon : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid CouponId { get; set; }
        public DateTime UsedAt { get; set; } = DateTime.UtcNow;
        public Guid? EnrollmentId { get; set; }
        public decimal DiscountAmount { get; set; }

        public virtual Coupon Coupon { get; set; }
        public virtual Enrollment Enrollment { get; set; }
    }
}