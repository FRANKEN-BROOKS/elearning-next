using System;
using System.Collections.Generic;
using Shared.Common.Entities;

namespace UserService.Core.Entities
{
    /// <summary>
    /// User entity
    /// </summary>
    public class User : AuditableEntity
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string ProfileImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsEmailVerified { get; set; } = false;
        public string EmailVerificationToken { get; set; }
        public string PasswordResetToken { get; set; }
        public DateTime? PasswordResetExpiry { get; set; }
        public DateTime? LastLoginDate { get; set; }

        // Navigation properties
        public virtual UserProfile Profile { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }

        // Computed property
        public string FullName => $"{FirstName} {LastName}";
    }
}