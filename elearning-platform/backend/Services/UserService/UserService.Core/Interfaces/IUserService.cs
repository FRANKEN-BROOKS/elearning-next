using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Common.DTOs;
using UserService.Core.DTOs;

namespace UserService.Core.Interfaces
{
    /// <summary>
    /// User service interface
    /// </summary>
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(Guid id);
        Task<UserDto> GetByEmailAsync(string email);
        Task<PaginatedResponse<UserDto>> GetAllAsync(PaginationRequest request);
        Task<UserDto> UpdateAsync(Guid id, UpdateUserRequestDto request);
        Task<UserDto> UpdateProfileAsync(Guid id, UpdateProfileRequestDto request);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ActivateAsync(Guid id);
        Task<bool> DeactivateAsync(Guid id);
        Task<bool> AssignRoleAsync(AssignRoleRequestDto request);
        Task<bool> RemoveRoleAsync(Guid userId, Guid roleId);
        Task<List<string>> GetUserRolesAsync(Guid userId);
        Task<List<string>> GetUserPermissionsAsync(Guid userId);
    }
}