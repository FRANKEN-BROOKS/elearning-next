using System;
using System.Collections.Generic;
using Shared.Common.Entities;

namespace CourseService.Core.Entities
{
    /// <summary>
    /// Course entity
    /// </summary>
    public class Course : AuditableEntity
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
        public Guid InstructorId { get; set; }
        public string ThumbnailUrl { get; set; }
        public string PreviewVideoUrl { get; set; }
        public string Level { get; set; } = "Beginner"; // Beginner, Intermediate, Advanced
        public string Language { get; set; } = "Thai";
        public decimal PriceThb { get; set; }
        public decimal? DiscountPriceThb { get; set; }
        public int Duration { get; set; } // in minutes
        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }
        public bool IsFeatured { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public int EnrollmentCount { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalRatings { get; set; }

        // Navigation properties
        public virtual Category Category { get; set; }
        public virtual ICollection<CourseTopic> Topics { get; set; }
        public virtual ICollection<Quiz> Quizzes { get; set; }
        public virtual ICollection<CourseReview> Reviews { get; set; }

        // Computed properties
        public decimal EffectivePrice => DiscountPriceThb ?? PriceThb;
        public bool HasDiscount => DiscountPriceThb.HasValue && DiscountPriceThb < PriceThb;
    }
}