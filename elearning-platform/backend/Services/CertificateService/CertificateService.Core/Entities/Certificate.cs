using System;
using Shared.Common.Entities;

namespace CertificateService.Core.Entities
{
    /// <summary>
    /// Certificate entity
    /// </summary>
    public class Certificate : BaseEntity
    {
        public string CertificateNumber { get; set; }
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public string UserFullName { get; set; }
        public string CourseTitle { get; set; }
        public string InstructorName { get; set; }
        public DateTime CompletionDate { get; set; }
        public DateTime IssueDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiryDate { get; set; }
        public decimal? FinalScore { get; set; }
        public string Grade { get; set; } // A, B, C, D, F or Pass/Fail
        public int TotalHours { get; set; }
        public string VerificationCode { get; set; }
        public string Status { get; set; } = "Active"; // Active, Revoked, Expired
        public string PdfUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public bool IsPublic { get; set; } = true;
        public DateTime? RevokedAt { get; set; }
        public Guid? RevokedBy { get; set; }
        public string RevokedReason { get; set; }
    }

    /// <summary>
    /// Certificate template entity
    /// </summary>
    public class CertificateTemplate : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string TemplateType { get; set; } // Course, Achievement, Participation
        public string BackgroundImageUrl { get; set; }
        public string LogoUrl { get; set; }
        public string SignatureUrl { get; set; }
        public string Layout { get; set; } // JSON configuration
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid CreatedBy { get; set; }
    }

    /// <summary>
    /// Certificate verification entity
    /// </summary>
    public class CertificateVerification : BaseEntity
    {
        public Guid CertificateId { get; set; }
        public string VerificationCode { get; set; }
        public string VerifiedBy { get; set; } // IP address or identifier
        public DateTime VerifiedAt { get; set; } = DateTime.UtcNow;

        public virtual Certificate Certificate { get; set; }
    }

    /// <summary>
    /// Certificate badge entity
    /// </summary>
    public class CertificateBadge : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string BadgeType { get; set; } // Achievement, Skill, Milestone
        public string IconUrl { get; set; }
        public string Criteria { get; set; } // JSON
        public int Points { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// User badge entity
    /// </summary>
    public class UserBadge : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid BadgeId { get; set; }
        public DateTime EarnedAt { get; set; } = DateTime.UtcNow;
        public bool IsDisplayed { get; set; } = true;

        public virtual CertificateBadge Badge { get; set; }
    }

    /// <summary>
    /// Certificate skill entity
    /// </summary>
    public class CertificateSkill : BaseEntity
    {
        public Guid CertificateId { get; set; }
        public string SkillName { get; set; }
        public string SkillLevel { get; set; } // Beginner, Intermediate, Advanced

        public virtual Certificate Certificate { get; set; }
    }

    /// <summary>
    /// Certificate share entity
    /// </summary>
    public class CertificateShare : BaseEntity
    {
        public Guid CertificateId { get; set; }
        public string Platform { get; set; } // LinkedIn, Facebook, Twitter, Email
        public DateTime SharedAt { get; set; } = DateTime.UtcNow;

        public virtual Certificate Certificate { get; set; }
    }
}