using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UserService.Core.DTOs
{
    /// <summary>
    /// User registration request DTO
    /// </summary>
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        public string LastName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string PhoneNumber { get; set; }
    }

    /// <summary>
    /// Login request DTO
    /// </summary>
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }

    /// <summary>
    /// Login response DTO
    /// </summary>
    public class LoginResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; }
    }

    /// <summary>
    /// User DTO
    /// </summary>
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string ProfileImageUrl { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Permissions { get; set; }
        public UserProfileDto Profile { get; set; }
    }

    /// <summary>
    /// User profile DTO
    /// </summary>
    public class UserProfileDto
    {
        public string Bio { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string LinkedInUrl { get; set; }
        public string WebsiteUrl { get; set; }
        public string Occupation { get; set; }
        public string Company { get; set; }
    }

    /// <summary>
    /// Update user request DTO
    /// </summary>
    public class UpdateUserRequestDto
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public string ProfileImageUrl { get; set; }
    }

    /// <summary>
    /// Update profile request DTO
    /// </summary>
    public class UpdateProfileRequestDto
    {
        [StringLength(1000)]
        public string Bio { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(100)]
        public string Province { get; set; }

        [StringLength(20)]
        public string PostalCode { get; set; }

        [StringLength(100)]
        public string Country { get; set; }

        [Url]
        public string LinkedInUrl { get; set; }

        [Url]
        public string WebsiteUrl { get; set; }

        [StringLength(100)]
        public string Occupation { get; set; }

        [StringLength(100)]
        public string Company { get; set; }
    }

    /// <summary>
    /// Change password request DTO
    /// </summary>
    public class ChangePasswordRequestDto
    {
        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string NewPassword { get; set; }
    }

    /// <summary>
    /// Refresh token request DTO
    /// </summary>
    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; }
    }

    /// <summary>
    /// Forgot password request DTO
    /// </summary>
    public class ForgotPasswordRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    /// <summary>
    /// Reset password request DTO
    /// </summary>
    public class ResetPasswordRequestDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; }
    }

    /// <summary>
    /// Role DTO
    /// </summary>
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public List<string> Permissions { get; set; }
    }

    /// <summary>
    /// Assign role request DTO
    /// </summary>
    public class AssignRoleRequestDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid RoleId { get; set; }
    }
}