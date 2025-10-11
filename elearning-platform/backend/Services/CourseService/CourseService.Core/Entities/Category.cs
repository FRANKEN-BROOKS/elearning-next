using System.Collections.Generic;
using Shared.Common.Entities;

namespace CourseService.Core.Entities
{
    /// <summary>
    /// Course category entity
    /// </summary>
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Course> Courses { get; set; }
    }
}