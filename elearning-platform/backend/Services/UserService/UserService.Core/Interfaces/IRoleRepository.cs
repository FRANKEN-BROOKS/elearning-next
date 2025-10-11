using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserService.Core.Entities;

namespace UserService.Core.Interfaces
{
    /// <summary>
    /// Role repository interface
    /// </summary>
    public interface IRoleRepository
    {
        Task<Role> GetByIdAsync(Guid id);
        Task<Role> GetByNameAsync(string name);
        Task<Role> GetByIdWithPermissionsAsync(Guid id);
        Task<List<Role>> GetAllAsync();
        Task<Role> CreateAsync(Role role);
        Task<Role> UpdateAsync(Role role);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task AssignPermissionToRoleAsync(Guid roleId, Guid permissionId, Guid? grantedBy = null);
        Task RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId);
        Task<List<Permission>> GetRolePermissionsAsync(Guid roleId);
    }
}