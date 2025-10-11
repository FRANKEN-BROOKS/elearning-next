using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Common.DTOs;
using Shared.Common.Exceptions;
using UserService.Core.DTOs;
using UserService.Core.Entities;
using UserService.Core.Interfaces;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Services
{
    /// <summary>
    /// User service implementation
    /// </summary>
    public class UserServiceImpl : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly UserDbContext _context;

        public UserServiceImpl(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            UserDbContext context)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _context = context;
        }

        public async Task<UserDto> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(id);

            if (user == null)
            {
                throw new NotFoundException("User", id);
            }

            return await MapToUserDto(user);
        }

        public async Task<UserDto> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
            {
                throw new NotFoundException($"User with email '{email}' not found");
            }

            var userWithRoles = await _userRepository.GetByIdWithRolesAsync(user.Id);
            return await MapToUserDto(userWithRoles);
        }

        public async Task<PaginatedResponse<UserDto>> GetAllAsync(PaginationRequest request)
        {
            var users = await _userRepository.GetAllAsync(
                request.PageNumber, 
                request.PageSize, 
                request.SearchTerm
            );

            var totalCount = await _userRepository.GetTotalCountAsync(request.SearchTerm);

            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                var userWithRoles = await _userRepository.GetByIdWithRolesAsync(user.Id);
                userDtos.Add(await MapToUserDto(userWithRoles));
            }

            return PaginatedResponse<UserDto>.Create(
                userDtos,
                request.PageNumber,
                request.PageSize,
                totalCount
            );
        }

        public async Task<UserDto> UpdateAsync(Guid id, UpdateUserRequestDto request)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException("User", id);
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.DateOfBirth = request.DateOfBirth;
            user.ProfileImageUrl = request.ProfileImageUrl;

            await _userRepository.UpdateAsync(user);

            var updatedUser = await _userRepository.GetByIdWithRolesAsync(id);
            return await MapToUserDto(updatedUser);
        }

        public async Task<UserDto> UpdateProfileAsync(Guid id, UpdateProfileRequestDto request)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException("User", id);
            }

            if (user.Profile == null)
            {
                user.Profile = new UserProfile { UserId = id };
            }

            user.Profile.Bio = request.Bio;
            user.Profile.Address = request.Address;
            user.Profile.City = request.City;
            user.Profile.Province = request.Province;
            user.Profile.PostalCode = request.PostalCode;
            user.Profile.Country = request.Country ?? "Thailand";
            user.Profile.LinkedInUrl = request.LinkedInUrl;
            user.Profile.WebsiteUrl = request.WebsiteUrl;
            user.Profile.Occupation = request.Occupation;
            user.Profile.Company = request.Company;
            user.Profile.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            var updatedUser = await _userRepository.GetByIdWithRolesAsync(id);
            return await MapToUserDto(updatedUser);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var exists = await _userRepository.ExistsAsync(id);

            if (!exists)
            {
                throw new NotFoundException("User", id);
            }

            await _userRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> ActivateAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException("User", id);
            }

            user.IsActive = true;
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> DeactivateAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException("User", id);
            }

            user.IsActive = false;
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> AssignRoleAsync(AssignRoleRequestDto request)
        {
            var userExists = await _userRepository.ExistsAsync(request.UserId);
            if (!userExists)
            {
                throw new NotFoundException("User", request.UserId);
            }

            var roleExists = await _roleRepository.ExistsAsync(request.RoleId);
            if (!roleExists)
            {
                throw new NotFoundException("Role", request.RoleId);
            }

            var userRole = new UserRole
            {
                UserId = request.UserId,
                RoleId = request.RoleId,
                AssignedAt = DateTime.UtcNow
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveRoleAsync(Guid userId, Guid roleId)
        {
            var userRole = await _context.UserRoles
                .FindAsync(userId, roleId);

            if (userRole == null)
            {
                throw new NotFoundException("User role assignment not found");
            }

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<string>> GetUserRolesAsync(Guid userId)
        {
            return await _userRepository.GetUserRolesAsync(userId);
        }

        public async Task<List<string>> GetUserPermissionsAsync(Guid userId)
        {
            return await _userRepository.GetUserPermissionsAsync(userId);
        }

        private async Task<UserDto> MapToUserDto(User user)
        {
            var roles = await _userRepository.GetUserRolesAsync(user.Id);
            var permissions = await _userRepository.GetUserPermissionsAsync(user.Id);

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                ProfileImageUrl = user.ProfileImageUrl,
                IsActive = user.IsActive,
                IsEmailVerified = user.IsEmailVerified,
                LastLoginDate = user.LastLoginDate,
                CreatedAt = user.CreatedAt,
                Roles = roles,
                Permissions = permissions,
                Profile = user.Profile != null ? new UserProfileDto
                {
                    Bio = user.Profile.Bio,
                    Address = user.Profile.Address,
                    City = user.Profile.City,
                    Province = user.Profile.Province,
                    PostalCode = user.Profile.PostalCode,
                    Country = user.Profile.Country,
                    LinkedInUrl = user.Profile.LinkedInUrl,
                    WebsiteUrl = user.Profile.WebsiteUrl,
                    Occupation = user.Profile.Occupation,
                    Company = user.Profile.Company
                } : null
            };
        }
    }
}