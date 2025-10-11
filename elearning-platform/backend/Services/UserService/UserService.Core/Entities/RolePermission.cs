using System;

namespace UserService.Core.Entities
{
    /// <summary>
    /// Role-Permission relationship entity
    /// </summary>
    public class RolePermission
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
        public Guid? GrantedBy { get; set; }

        // Navigation properties
        public virtual Role Role { get; set; }
        public virtual Permission Permission { get; set; }
    }
}