using AmazonClone.Application.Common;
using AmazonClone.Application.Features.Auth;
using AmazonClone.Application.Features.Auth.DTOs;
using AmazonClone.Application.Features.Auth.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AmazonClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
    }
}
