using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserService.Core.Entities;

namespace UserService.Core.Interfaces
{
    /// <summary>
    /// User repository interface
    /// </summary>
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByIdWithRolesAsync(Guid id);
        Task<List<User>> GetAllAsync(int pageNumber, int pageSize, string searchTerm = null);
        Task<int> GetTotalCountAsync(string searchTerm = null);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> EmailExistsAsync(string email);
        Task<List<string>> GetUserRolesAsync(Guid userId);
        Task<List<string>> GetUserPermissionsAsync(Guid userId);
    }
}