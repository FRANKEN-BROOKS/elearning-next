using UserService.Core.Entities;

namespace UserService.Core.Interfaces
{
    public interface IUserRoleRepository
    {
        Task<UserRole> CreateAsync(UserRole userRole);
        Task<IEnumerable<UserRole>> GetByUserIdAsync(Guid userId);
        Task DeleteAsync(Guid userId, Guid roleId);
        Task<bool> UserHasRoleAsync(Guid userId, string roleName);
    }
}