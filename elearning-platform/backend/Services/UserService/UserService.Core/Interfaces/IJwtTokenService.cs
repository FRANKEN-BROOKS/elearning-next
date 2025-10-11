using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace UserService.Core.Interfaces
{
    /// <summary>
    /// JWT token service interface
    /// </summary>
    public interface IJwtTokenService
    {
        string GenerateAccessToken(Guid userId, string email, List<string> roles, List<string> permissions);
        string GenerateRefreshToken();
        ClaimsPrincipal ValidateToken(string token);
        Guid GetUserIdFromToken(string token);
    }
}