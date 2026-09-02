using AmazonClone.Application.Common;
using AmazonClone.Application.Features.Auth;
using AmazonClone.Application.Features.Auth.DTOs;
using AmazonClone.Application.Features.Auth.Interfaces;
using AmazonClone.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = Roles.Customer)]
        [HttpGet("custommer-test")]
        public IActionResult CustomerTest()
        {
            return Ok(new
            {
                Success = true,
                Message = "Customer role authorization working."
            });
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
        [SwaggerOperation(Summary = "Forgot password",
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
            if (!result)
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
        //----------------------
        // Verify Email
        //-----------------------
        [HttpGet("verify-email")]
        [SwaggerOperation(
            Summary = "Verify email address",
            Description = "Confirms the user's email address using the verification token.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Email verified successfully,")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid user ID or verification token.")]
        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            var result = await _authService.VerifyEmailAsync(userId, token);
            if (!result)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid user ID or verification token."
                });
            }
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Email verified successfully.",
                Data = null
            });
        }
        //----------------------
        // Generate OTP
        //----------------------
        [HttpPost("generate-otp")]
        [SwaggerOperation(
            Summary = "Generate OTP",
            Description = "Generates a 6-digit OTP for login and keeps it valid for 5 minutes."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "OTP generated successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "User not found.")]
        public async Task<IActionResult> GenerateOtp(GenerateOtpDto dto)
        {
            var otp = await _authService.GenerateOtpAsync(dto);
            if (string.IsNullOrEmpty(otp))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not found."
                });
            }
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "OTP generated successfully.",
                Data = otp
            });
        }
        //----------------------
        // Verify OTP
        //----------------------
        [HttpPost("verify-otp")]
        [SwaggerOperation(
            Summary = "Verify OTP",
            Description = "Verifies the OTP and completes OTP authentication."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "OTP verified successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid or expired OTP.")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
        {
            var result = await _authService.VerifyOtpAsync(dto);
            if (!result.IsSuccess)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = result.Message
                });
            }
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = result.Message,
                Data = result
            });

        }
        //----------------------
        // Refresh Token
        //----------------------
        [HttpPost("refresh-token")]
        [SwaggerOperation(
            Summary = "Refresh access token",
            Description = "Generates a new JWT access token and refresh token using a valid refresh token."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Tokens refreshed successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid or expired refresh token.")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto);
            if(!result.IsSuccess)
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
        [HttpPost("generate-email-confirmation-token")]
        [SwaggerOperation(
            Summary = "Generate email confirmation token",
            Description = "Generates an email confirmation token for testing the email verification flow."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Token generated successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "User not found.")]
        public async Task<IActionResult> GenerateEmailConfirmationToken(string email)
        {
            var token = await _authService.GenerateEmailConfirmationTokenAsync(email);

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not found."
                });
            }

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Email confirmation token generated successfully.",
                Data = token
            });
        }
    }
}
