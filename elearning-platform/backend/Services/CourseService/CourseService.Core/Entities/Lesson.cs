using System;
using System.Collections.Generic;
using Shared.Common.Entities;

namespace CourseService.Core.Entities
{
    /// <summary>
    /// Lesson entity
    /// </summary>
    public class Lesson : BaseEntity
    {
        public Guid TopicId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string VideoUrl { get; set; }
        public int Duration { get; set; } // in minutes
        public int DisplayOrder { get; set; }
        public bool IsFree { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual CourseTopic Topic { get; set; }
        public virtual ICollection<LessonResource> Resources { get; set; }
    }
}