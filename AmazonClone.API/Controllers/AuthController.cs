using AmazonClone.Application.Common;
using AmazonClone.Application.Features.Auth;
using AmazonClone.Application.Features.Auth.DTOs;
using AmazonClone.Application.Features.Auth.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AmazonClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Authentication APIs")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        //----------------------
        // Register
        //-----------------------
        [HttpPost("register")]
        [SwaggerOperation(
            Summary = "Register a new user",
            Description = "Creates a new customer account."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Registration successful.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (!result.IsSuccess)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = result.Message
                });
            }
            return Ok(new ApiResponse<AuthResponseDto>
            {
                Success = result.IsSuccess,
                Message = result.Message,
                Data = result
            });
        }
        //----------------------
        // Login
        //----------------------
        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "Login",
            Description = "Authenticates the user and returns a JWT token."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Login successful.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid credentials.")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (!result.IsSuccess)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = result.Message
                });
            }
            return Ok(new ApiResponse<AuthResponseDto>
            {
                Success = true,
                Message = result.Message,
                Data = result
            });
        }
        //--------------------
        // Forgot Password
        //--------------------
        [HttpPost("forgot-password")]
        [SwaggerOperation(Summary ="Forgot password",
            Description = "Generates a password reset token for the specified email address.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Password reset token generated.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            var token = await _authService.ForgotPasswordAsync(dto);
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Password reset token generated successfully.",
                Data = token
            });
        }
        //----------------------
        // Reset Password
        //----------------------
        [HttpPost("reset-password")]
        [SwaggerOperation(
            Summary = "Reset password",
            Description = "Resets the user's password using a valid password reset token."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Password reset successful.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid or expired reset token.")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var result = await _authService.ResetPasswordAsync(dto);
            if(!result)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid or expired reset token, or passwords do not match."
                });
            }
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Password reset successfully,",
                Data = null
            });
        }
    }
}
