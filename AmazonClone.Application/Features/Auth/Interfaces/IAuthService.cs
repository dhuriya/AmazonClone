using AmazonClone.Application.Features.Auth.DTOs;
using AmazonClone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Application.Features.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<string> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
        Task<bool> VerifyEmailAsync(string userId, string token);
        Task<string> GenerateOtpAsync(GenerateOtpDto dto);
        Task<AuthResponseDto> VerifyOtpAsync(VerifyOtpDto dto);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto);
        Task<AuthResponseDto> GenerateAuthTokensAsync(ApplicationUser user);
        Task<string> GenerateEmailConfirmationTokenAsync(string email);
    }
}
