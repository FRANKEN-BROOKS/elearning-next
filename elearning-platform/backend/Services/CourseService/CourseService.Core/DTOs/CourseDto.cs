using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseService.Core.DTOs
{
    /// <summary>
    /// Create course request DTO
    /// </summary>
    public class CreateCourseRequestDto
    {
        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [StringLength(500)]
        public string ShortDescription { get; set; }

        public string Description { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        public Guid InstructorId { get; set; }

        public string ThumbnailUrl { get; set; }
        public string PreviewVideoUrl { get; set; }

        [Required]
        public string Level { get; set; } = "Beginner";

        public string Language { get; set; } = "Thai";

        [Required]
        [Range(0, double.MaxValue)]
        public decimal PriceThb { get; set; }

        public decimal? DiscountPriceThb { get; set; }
    }

    /// <summary>
    /// Update course request DTO
    /// </summary>
    public class UpdateCourseRequestDto
    {
        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [StringLength(500)]
        public string ShortDescription { get; set; }

        public string Description { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        public string ThumbnailUrl { get; set; }
        public string PreviewVideoUrl { get; set; }
        public string Level { get; set; }
        public string Language { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PriceThb { get; set; }

        public decimal? DiscountPriceThb { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
    }

    /// <summary>
    /// Course DTO
    /// </summary>
    public class CourseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Guid InstructorId { get; set; }
        public string InstructorName { get; set; }
        public string ThumbnailUrl { get; set; }
        public string PreviewVideoUrl { get; set; }
        public string Level { get; set; }
        public string Language { get; set; }
        public decimal PriceThb { get; set; }
        public decimal? DiscountPriceThb { get; set; }
        public decimal EffectivePrice { get; set; }
        public bool HasDiscount { get; set; }
        public int Duration { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }
        public bool IsFeatured { get; set; }
        public int EnrollmentCount { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<CourseTopicDto> Topics { get; set; }
    }

    /// <summary>
    /// Create topic request DTO
    /// </summary>
    public class CreateTopicRequestDto
    {
        [Required]
        public Guid CourseId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public int DisplayOrder { get; set; }
    }

    /// <summary>
    /// Course topic DTO
    /// </summary>
    public class CourseTopicDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public int Duration { get; set; }
        public bool IsActive { get; set; }
        public List<LessonDto> Lessons { get; set; }
    }

    /// <summary>
    /// Create lesson request DTO
    /// </summary>
    public class CreateLessonRequestDto
    {
        [Required]
        public Guid TopicId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        public string Content { get; set; }
        public string VideoUrl { get; set; }

        [Required]
        public int Duration { get; set; }

        public int DisplayOrder { get; set; }
        public bool IsFree { get; set; }
    }

    /// <summary>
    /// Lesson DTO
    /// </summary>
    public class LessonDto
    {
        public Guid Id { get; set; }
        public Guid TopicId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string VideoUrl { get; set; }
        public int Duration { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsFree { get; set; }
        public bool IsActive { get; set; }
        public List<LessonResourceDto> Resources { get; set; }
    }

    /// <summary>
    /// Lesson resource DTO
    /// </summary>
    public class LessonResourceDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ResourceType { get; set; }
        public string ResourceUrl { get; set; }
        public long? FileSize { get; set; }
    }

    /// <summary>
    /// Create quiz request DTO
    /// </summary>
    public class CreateQuizRequestDto
    {
        [Required]
        public Guid CourseId { get; set; }

        public Guid? TopicId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Range(0, 100)]
        public decimal PassingScore { get; set; } = 70.00m;

        public int? TimeLimit { get; set; }
        public int? MaxAttempts { get; set; }
    }

    /// <summary>
    /// Quiz DTO
    /// </summary>
    public class QuizDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public Guid? TopicId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal PassingScore { get; set; }
        public int? TimeLimit { get; set; }
        public int? MaxAttempts { get; set; }
        public bool IsActive { get; set; }
        public List<QuestionDto> Questions { get; set; }
    }

    /// <summary>
    /// Create question request DTO
    /// </summary>
    public class CreateQuestionRequestDto
    {
        [Required]
        public Guid QuizId { get; set; }

        [Required]
        public string QuestionText { get; set; }

        [Required]
        public string QuestionType { get; set; }

        public int Points { get; set; } = 1;
        public string Explanation { get; set; }
        public string ImageUrl { get; set; }

        [Required]
        public List<CreateQuestionOptionDto> Options { get; set; }
    }

    /// <summary>
    /// Question DTO
    /// </summary>
    public class QuestionDto
    {
        public Guid Id { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public int Points { get; set; }
        public string Explanation { get; set; }
        public string ImageUrl { get; set; }
        public List<QuestionOptionDto> Options { get; set; }
    }

    /// <summary>
    /// Create question option DTO
    /// </summary>
    public class CreateQuestionOptionDto
    {
        [Required]
        public string OptionText { get; set; }

        public bool IsCorrect { get; set; }
        public int DisplayOrder { get; set; }
    }

    /// <summary>
    /// Question option DTO
    /// </summary>
    public class QuestionOptionDto
    {
        public Guid Id { get; set; }
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; }
    }

    /// <summary>
    /// Submit quiz answer request DTO
    /// </summary>
    public class SubmitQuizAnswerRequestDto
    {
        [Required]
        public Guid QuizId { get; set; }

        [Required]
        public List<QuizAnswerDto> Answers { get; set; }
    }

    /// <summary>
    /// Quiz answer DTO
    /// </summary>
    public class QuizAnswerDto
    {
        [Required]
        public Guid QuestionId { get; set; }

        public Guid? SelectedOptionId { get; set; }
        public string AnswerText { get; set; }
    }

    /// <summary>
    /// Quiz result DTO
    /// </summary>
    public class QuizResultDto
    {
        public Guid AttemptId { get; set; }
        public decimal Score { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public bool IsPassed { get; set; }
        public int TimeSpent { get; set; }
        public List<QuestionResultDto> QuestionResults { get; set; }
    }

    /// <summary>
    /// Question result DTO
    /// </summary>
    public class QuestionResultDto
    {
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; }
        public bool IsCorrect { get; set; }
        public int PointsAwarded { get; set; }
        public string YourAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public string Explanation { get; set; }
    }

    /// <summary>
    /// Category DTO
    /// </summary>
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public int CourseCount { get; set; }
    }
}