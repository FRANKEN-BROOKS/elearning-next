using System;
using System.Collections.Generic;

namespace UserService.Core.Entities
{
    /// <summary>
    /// Permission entity
    /// </summary>
    public class Permission
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}