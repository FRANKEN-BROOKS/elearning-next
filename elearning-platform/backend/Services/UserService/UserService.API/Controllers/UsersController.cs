using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common.DTOs;
using UserService.Core.DTOs;
using UserService.Core.Interfaces;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get all users with pagination
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PaginatedResponse<UserDto>>> GetAll([FromQuery] PaginationRequest request)
        {
            try
            {
                var result = await _userService.GetAllAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<UserDto>>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetById(Guid id)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
                var isAdmin = User.IsInRole("Admin");

                // Users can only view their own profile unless they're admin
                if (userId != id && !isAdmin)
                {
                    return Forbid();
                }

                var result = await _userService.GetByIdAsync(id);
                return Ok(ApiResponse<UserDto>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResponse(ex.Message, 404));
            }
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        [HttpGet("me")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetCurrentUser()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
                var result = await _userService.GetByIdAsync(userId);
                return Ok(ApiResponse<UserDto>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResponse(ex.Message, 404));
            }
        }

        /// <summary>
        /// Update user
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> Update(Guid id, [FromBody] UpdateUserRequestDto request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
                var isAdmin = User.IsInRole("Admin");

                // Users can only update their own profile unless they're admin
                if (userId != id && !isAdmin)
                {
                    return Forbid();
                }

                var result = await _userService.UpdateAsync(id, request);
                return Ok(ApiResponse<UserDto>.SuccessResponse(result, "User updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<UserDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        [HttpPut("{id}/profile")]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateProfile(Guid id, [FromBody] UpdateProfileRequestDto request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
                var isAdmin = User.IsInRole("Admin");

                // Users can only update their own profile unless they're admin
                if (userId != id && !isAdmin)
                {
                    return Forbid();
                }

                var result = await _userService.UpdateProfileAsync(id, request);
                return Ok(ApiResponse<UserDto>.SuccessResponse(result, "Profile updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<UserDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Delete user
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            try
            {
                var result = await _userService.DeleteAsync(id);
                return Ok(ApiResponse<bool>.SuccessResponse(result, "User deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Activate user
        /// </summary>
        [HttpPost("{id}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> Activate(Guid id)
        {
            try
            {
                var result = await _userService.ActivateAsync(id);
                return Ok(ApiResponse<bool>.SuccessResponse(result, "User activated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Deactivate user
        /// </summary>
        [HttpPost("{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> Deactivate(Guid id)
        {
            try
            {
                var result = await _userService.DeactivateAsync(id);
                return Ok(ApiResponse<bool>.SuccessResponse(result, "User deactivated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Assign role to user
        /// </summary>
        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> AssignRole([FromBody] AssignRoleRequestDto request)
        {
            try
            {
                var result = await _userService.AssignRoleAsync(request);
                return Ok(ApiResponse<bool>.SuccessResponse(result, "Role assigned successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Remove role from user
        /// </summary>
        [HttpDelete("{userId}/roles/{roleId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> RemoveRole(Guid userId, Guid roleId)
        {
            try
            {
                var result = await _userService.RemoveRoleAsync(userId, roleId);
                return Ok(ApiResponse<bool>.SuccessResponse(result, "Role removed successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Get user roles
        /// </summary>
        [HttpGet("{id}/roles")]
        public async Task<ActionResult<ApiResponse<List<string>>>> GetUserRoles(Guid id)
        {
            try
            {
                var result = await _userService.GetUserRolesAsync(id);
                return Ok(ApiResponse<List<string>>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<string>>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Get user permissions
        /// </summary>
        [HttpGet("{id}/permissions")]
        public async Task<ActionResult<ApiResponse<List<string>>>> GetUserPermissions(Guid id)
        {
            try
            {
                var result = await _userService.GetUserPermissionsAsync(id);
                return Ok(ApiResponse<List<string>>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<string>>.ErrorResponse(ex.Message));
            }
        }
    }
}