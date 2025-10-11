using System;
using System.Threading.Tasks;
using UserService.Core.Entities;

namespace UserService.Core.Interfaces
{
    /// <summary>
    /// Refresh token repository interface
    /// </summary>
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetByTokenAsync(string token);
        Task<RefreshToken> CreateAsync(RefreshToken refreshToken);
        Task<RefreshToken> UpdateAsync(RefreshToken refreshToken);
        Task RevokeAsync(string token);
        Task RevokeAllUserTokensAsync(Guid userId);
        Task DeleteExpiredTokensAsync();
    }
}