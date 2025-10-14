using Microsoft.EntityFrameworkCore;
using UserService.Core.Entities;
using UserService.Core.Interfaces;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly UserDbContext _context;

        public UserRoleRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<UserRole> CreateAsync(UserRole userRole)
        {
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
            return userRole;
        }

        public async Task<IEnumerable<UserRole>> GetByUserIdAsync(Guid userId)
        {
            return await _context.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .ToListAsync();
        }

        public async Task DeleteAsync(Guid userId, Guid roleId)
        {
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
            
            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> UserHasRoleAsync(Guid userId, string roleName)
        {
            return await _context.UserRoles
                .Include(ur => ur.Role)
                .AnyAsync(ur => ur.UserId == userId && ur.Role.Name == roleName);
        }
    }
}