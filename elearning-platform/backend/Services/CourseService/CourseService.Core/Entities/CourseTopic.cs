using System;
using System.Collections.Generic;
using Shared.Common.Entities;

namespace CourseService.Core.Entities
{
    /// <summary>
    /// Course topic/section entity
    /// </summary>
    public class CourseTopic : BaseEntity
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public int Duration { get; set; } // in minutes
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Course Course { get; set; }
        public virtual ICollection<Lesson> Lessons { get; set; }
        public virtual ICollection<Quiz> Quizzes { get; set; }
    }
}