using System;
using System.Threading.Tasks;
using BCrypt.Net;
using Shared.Common.Exceptions;
using Shared.MessageQueue;
using Shared.MessageQueue.Events;
using UserService.Core.DTOs;
using UserService.Core.Entities;
using UserService.Core.Interfaces;

namespace UserService.Infrastructure.Services
{
    /// <summary>
    /// Authentication service implementation
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IMessageQueueService _messageQueue;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUserRoleRepository _userRoleRepository;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IUserProfileRepository userProfileRepository,
            IUserRoleRepository userRoleRepository,
            IJwtTokenService jwtTokenService,
            IMessageQueueService messageQueue)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _userProfileRepository = userProfileRepository;
            _userRoleRepository = userRoleRepository;
            _jwtTokenService = jwtTokenService;
            _messageQueue = messageQueue;
        }

        public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            try
            {
                // Check if email already exists
                if (await _userRepository.EmailExistsAsync(request.Email))
                {
                    throw new ConflictException("Email already exists");
                }

                // Hash password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Create user
                var user = new User
                {
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    IsActive = true,
                    IsEmailVerified = false,
                    EmailVerificationToken = Guid.NewGuid().ToString()
                };

                user = await _userRepository.CreateAsync(user);

                // Create user profile
                // user.Profile = new UserProfile
                // {
                //     UserId = user.Id,
                //     Country = "Thailand"
                // };

                var profile = new UserProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Country = "Thailand",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _userProfileRepository.CreateAsync(profile);

                

                // Get Student role
                var studentRole = await _roleRepository.GetByNameAsync("Student");
                if (studentRole == null)
                {
                    throw new InvalidOperationException("Default 'Student' role not found.");
                }

                // Assign role using repository
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = studentRole.Id,
                    AssignedAt = DateTime.UtcNow
                };
                await _userRoleRepository.CreateAsync(userRole);


                // Publish user registered event
                await _messageQueue.PublishAsync("user.registered", new UserRegisteredEvent
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                });

                // Generate tokens
                return await GenerateTokensAsync(user);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            // Get user by email
            var user = await _userRepository.GetByIdWithRolesAsync(
                (await _userRepository.GetByEmailAsync(request.Email))?.Id ?? Guid.Empty
            );

            if (user == null)
            {
                throw new UnauthorizedException("Invalid email or password");
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedException("Invalid email or password");
            }

            // Check if user is active
            if (!user.IsActive)
            {
                throw new UnauthorizedException("Account is inactive");
            }

            // Update last login date
            user.LastLoginDate = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            // Generate tokens
            return await GenerateTokensAsync(user);
        }

        public async Task<LoginResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (token == null || !token.IsValid)
            {
                throw new UnauthorizedException("Invalid refresh token");
            }

            // Get user with roles
            var user = await _userRepository.GetByIdWithRolesAsync(token.UserId);

            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedException("User not found or inactive");
            }

            // Revoke old token
            await _refreshTokenRepository.RevokeAsync(refreshToken);

            // Generate new tokens
            return await GenerateTokensAsync(user);
        }

        public async Task LogoutAsync(Guid userId)
        {
            await _refreshTokenRepository.RevokeAllUserTokensAsync(userId);
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException("User", userId);
            }

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                throw new ValidationException("Current password is incorrect");
            }

            // Hash new password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _userRepository.UpdateAsync(user);

            // Revoke all refresh tokens
            await _refreshTokenRepository.RevokeAllUserTokensAsync(userId);

            return true;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                // Don't reveal that user doesn't exist
                return true;
            }

            // Generate reset token
            user.PasswordResetToken = Guid.NewGuid().ToString();
            user.PasswordResetExpiry = DateTime.UtcNow.AddHours(1);
            await _userRepository.UpdateAsync(user);

            // Publish email notification event
            await _messageQueue.PublishAsync("email.notification", new EmailNotificationEvent
            {
                To = user.Email,
                Subject = "Password Reset Request",
                TemplateName = "PasswordReset",
                TemplateData = new { ResetToken = user.PasswordResetToken, UserName = user.FullName }
            });

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Token);

            if (user == null || 
                user.PasswordResetToken != request.Token || 
                user.PasswordResetExpiry < DateTime.UtcNow)
            {
                throw new ValidationException("Invalid or expired reset token");
            }

            // Hash new password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetExpiry = null;
            await _userRepository.UpdateAsync(user);

            // Revoke all refresh tokens
            await _refreshTokenRepository.RevokeAllUserTokensAsync(user.Id);

            return true;
        }

        public async Task<bool> VerifyEmailAsync(string token)
        {
            var user = await _userRepository.GetByEmailAsync(token);

            if (user == null || user.EmailVerificationToken != token)
            {
                throw new ValidationException("Invalid verification token");
            }

            user.IsEmailVerified = true;
            user.EmailVerificationToken = null;
            await _userRepository.UpdateAsync(user);

            return true;
        }

        private async Task<LoginResponseDto> GenerateTokensAsync(User user)
        {
            var roles = await _userRepository.GetUserRolesAsync(user.Id);
            var permissions = await _userRepository.GetUserPermissionsAsync(user.Id);

            // Generate access token
            var accessToken = _jwtTokenService.GenerateAccessToken(
                user.Id, 
                user.Email, 
                roles, 
                permissions
            );

            // Generate refresh token
            var refreshTokenString = _jwtTokenService.GenerateRefreshToken();
            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenString,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsActive = true
            };

            await _refreshTokenRepository.CreateAsync(refreshToken);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenString,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    ProfileImageUrl = user.ProfileImageUrl,
                    IsActive = user.IsActive,
                    IsEmailVerified = user.IsEmailVerified,
                    Roles = roles,
                    Permissions = permissions
                }
            };
        }
    }
}