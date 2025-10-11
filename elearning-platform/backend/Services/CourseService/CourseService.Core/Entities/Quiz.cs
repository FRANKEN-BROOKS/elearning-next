using System;
using System.Collections.Generic;
using Shared.Common.Entities;

namespace CourseService.Core.Entities
{
    /// <summary>
    /// Quiz entity
    /// </summary>
    public class Quiz : BaseEntity
    {
        public Guid CourseId { get; set; }
        public Guid? TopicId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal PassingScore { get; set; } = 70.00m;
        public int? TimeLimit { get; set; } // in minutes
        public int? MaxAttempts { get; set; }
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; }

        // Navigation properties
        public virtual Course Course { get; set; }
        public virtual CourseTopic Topic { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }

    /// <summary>
    /// Question entity
    /// </summary>
    public class Question : BaseEntity
    {
        public Guid QuizId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; } // MultipleChoice, TrueFalse, ShortAnswer
        public int Points { get; set; } = 1;
        public int DisplayOrder { get; set; }
        public string Explanation { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Quiz Quiz { get; set; }
        public virtual ICollection<QuestionOption> Options { get; set; }
    }

    /// <summary>
    /// Question option entity
    /// </summary>
    public class QuestionOption : BaseEntity
    {
        public Guid QuestionId { get; set; }
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; }
        public int DisplayOrder { get; set; }

        // Navigation properties
        public virtual Question Question { get; set; }
    }

    /// <summary>
    /// Lesson resource entity
    /// </summary>
    public class LessonResource : BaseEntity
    {
        public Guid LessonId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ResourceType { get; set; } // PDF, Video, Link, Document
        public string ResourceUrl { get; set; }
        public long? FileSize { get; set; } // in bytes
        public int DisplayOrder { get; set; }

        // Navigation properties
        public virtual Lesson Lesson { get; set; }
    }

    /// <summary>
    /// Course review entity
    /// </summary>
    public class CourseReview : BaseEntity
    {
        public Guid CourseId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; } // 1-5
        public string Comment { get; set; }
        public bool IsApproved { get; set; }

        // Navigation properties
        public virtual Course Course { get; set; }
    }

    /// <summary>
    /// User progress entity
    /// </summary>
    public class UserProgress : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public Guid? LessonId { get; set; }
        public decimal CompletionPercentage { get; set; }
        public DateTime LastAccessedAt { get; set; } = DateTime.UtcNow;
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Navigation properties
        public virtual Course Course { get; set; }
        public virtual Lesson Lesson { get; set; }
    }

    /// <summary>
    /// Quiz attempt entity
    /// </summary>
    public class QuizAttempt : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid QuizId { get; set; }
        public decimal Score { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public bool IsPassed { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public int? TimeSpent { get; set; } // in seconds

        // Navigation properties
        public virtual Quiz Quiz { get; set; }
        public virtual ICollection<QuizAnswer> Answers { get; set; }
    }

    /// <summary>
    /// Quiz answer entity
    /// </summary>
    public class QuizAnswer : BaseEntity
    {
        public Guid AttemptId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid? SelectedOptionId { get; set; }
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
        public int PointsAwarded { get; set; }
        public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual QuizAttempt Attempt { get; set; }
        public virtual Question Question { get; set; }
        public virtual QuestionOption SelectedOption { get; set; }
    }
}