using UserService.Core.Entities;

namespace UserService.Core.Interfaces
{
    public interface IUserProfileRepository
    {
        Task<UserProfile> CreateAsync(UserProfile profile);
        Task<UserProfile> UpdateAsync(UserProfile profile);
        Task<UserProfile?> GetByIdAsync(Guid id);
        Task<UserProfile?> GetByUserIdAsync(Guid userId);
        Task DeleteAsync(Guid id);
    }
}