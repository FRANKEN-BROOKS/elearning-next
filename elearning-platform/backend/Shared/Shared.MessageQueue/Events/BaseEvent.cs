using System;

namespace Shared.MessageQueue.Events
{
    /// <summary>
    /// Base event for all messages
    /// </summary>
    public abstract class BaseEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// User registered event
    /// </summary>
    public class UserRegisteredEvent : BaseEvent
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    /// <summary>
    /// Course enrolled event
    /// </summary>
    public class CourseEnrolledEvent : BaseEvent
    {
        public Guid EnrollmentId { get; set; }
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public string CourseTitle { get; set; }
        public decimal PriceThb { get; set; }
    }

    /// <summary>
    /// Course completed event
    /// </summary>
    public class CourseCompletedEvent : BaseEvent
    {
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string UserFullName { get; set; }
        public decimal? FinalScore { get; set; }
        public int TotalHours { get; set; }
    }

    /// <summary>
    /// Payment received event
    /// </summary>
    public class PaymentReceivedEvent : BaseEvent
    {
        public Guid PaymentId { get; set; }
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string TransactionId { get; set; }
    }

    /// <summary>
    /// Certificate issued event
    /// </summary>
    public class CertificateIssuedEvent : BaseEvent
    {
        public Guid CertificateId { get; set; }
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public string CertificateNumber { get; set; }
        public string PdfUrl { get; set; }
    }

    /// <summary>
    /// Email notification event
    /// </summary>
    public class EmailNotificationEvent : BaseEvent
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string TemplateName { get; set; }
        public object TemplateData { get; set; }
    }

    /// <summary>
    /// Course updated event
    /// </summary>
    public class CourseUpdatedEvent : BaseEvent
    {
        public Guid CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string UpdateType { get; set; } // NewLesson, PriceChange, ContentUpdate
        public string UpdateDetails { get; set; }
    }
}