using System.Collections.Generic;
using Shared.Common.Entities;

namespace UserService.Core.Entities
{
    /// <summary>
    /// Role entity
    /// </summary>
    public class Role : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}