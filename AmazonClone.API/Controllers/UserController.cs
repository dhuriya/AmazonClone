using AmazonClone.Application.Common;
using AmazonClone.Application.Features.Users.DTOs;
using AmazonClone.Application.Features.Users.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using System.Xml.Linq;

namespace AmazonClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [SwaggerTag("User APIs")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        //----------------------
        // Get Profile
        //----------------------
        [HttpGet("profile")]
        [SwaggerOperation(
            Summary = "Get current user profile",
            Description = "Returns the profile of the currently authenticated user."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Profile retrieved successfully.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authenticated.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User identity not found."
                });
            }
            var result = await _userService.GetProfileAsync(userId);
            if (result == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not found"
                });
            }
            return Ok(new ApiResponse<UserProfileDto>
            {
                Success = true,
                Message = "Profile retrieved successfully.",
                Data = result
            });
        }
        //----------------------
        // Update Profile
        //----------------------
        [HttpPut("profile")]
        [SwaggerOperation(
            Summary = "Update current user profile",
            Description = "Updates the profile of the currently authenticated user."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Profile updated successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Profile update failed.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authenticated.")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User identity not found."
                });
            }
            var result = await _userService.UpdateProfileAsync(userId, dto);
            if(!result)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Profile update failed."
                });
            }
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Profile updated successfully.",
                Data = null
            });
        }
        //----------------------
        // Change Password
        //----------------------
        [HttpPut("change-password")]
        [SwaggerOperation(
            Summary = "Change current user password",
            Description = "Changes the password of the currently authenticated user."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Password changed successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid current password or passwords do not match.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authenticated.")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "user identity not found."
                });
            }
            var result = await _userService.ChangePasswordAsync(userId, dto);
            if (!result)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid current password or passwords do not match."
                });
            }
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Password changed successfully.",
                Data = null
            });
        }
        //----------------------
        // Delete Account
        //----------------------
        [HttpDelete("account")]
        [SwaggerOperation(
            Summary = "Delete current user account",
            Description = "Permanently deletes the currently authenticated user account."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Account deleted successfully.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authenticated.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User identity not found."
                });
            }
            var result = await _userService.DeleteAccountAsync(userId);
            if(!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not found or account deletion failed."
                });
            }
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Account deleted successfully."
            });
        }
    }
}
