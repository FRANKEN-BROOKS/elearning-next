using System;
using Shared.Common.Entities;

namespace UserService.Core.Entities
{
    /// <summary>
    /// Refresh token entity for JWT token refresh
    /// </summary>
    public class RefreshToken : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string ReplacedByToken { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual User User { get; set; }

        // Computed properties
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt != null;
        public bool IsValid => IsActive && !IsExpired && !IsRevoked;
    }
}