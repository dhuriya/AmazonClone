using AmazonClone.Application.Features.Auth.DTOs;
using AmazonClone.Application.Features.Auth;
using AmazonClone.Application.Features.Auth.Interfaces;
using AmazonClone.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using AmazonClone.Shared.Constants;
using Azure.Core;
using Microsoft.EntityFrameworkCore;

namespace AmazonClone.Persistence.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Password and Confirm Password do not match."
                };
            }

            var user = new ApplicationUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserName = dto.Email,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            await _userManager.AddToRoleAsync(user, Roles.Customer);
            var verificationToke = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "User registered successfully.",
                EmailVerification = verificationToke,
                UserId = user.Id
            };
        }
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Invlid email or  password."
                };
            }
            if (!user.EmailConfirmed)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Please verify your email before logging in."
                };
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid email or password."
                };
            }
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!)
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            //var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //var token = new JwtSecurityToken(
            //    issuer: _configuration["Jwt:Issuer"],
            //    audience: _configuration["Jwt:Audience"],
            //    claims: claims,
            //    expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
            //    signingCredentials: creds
            // );
            //return new AuthResponseDto
            //{
            //    IsSuccess = true,
            //    Message = "Login Successfull.",
            //    Token = new JwtSecurityTokenHandler().WriteToken(token)
            //};
            var tokenResponse = await GenerateAuthTokensAsync(user);

            return new AuthResponseDto
            {
                IsSuccess = tokenResponse.IsSuccess,
                Message = "Login successful.",
                Token = tokenResponse.Token,
                RefreshToken = tokenResponse.RefreshToken
            };
        }
        public async Task<string> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return string.Empty;
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);
            await _userManager.UpdateAsync(user);

            return token;
        }
        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
            {
                return false;
            }
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return false;
            }
            if (user.PasswordResetToken != dto.Token)
            {
                return false;
            }
            if (user.PasswordResetTokenExpiry == null ||
                user.PasswordResetTokenExpiry < DateTime.Now)
            {
                return false;
            }
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user,
                resetToken, dto.NewPassword);
            if (!result.Succeeded)
            {
                return false;
            }
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;
            await _userManager.UpdateAsync(user);
            return true;
        }
        public async Task<bool> VerifyEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded;
        }
        public async Task<string> GenerateOtpAsync(GenerateOtpDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return string.Empty;
            }
            var random = new Random();
            var otp = random.Next(100000, 1000000).ToString();
            user.LoginOtp = otp;
            user.LoginOtpExpiry = DateTime.UtcNow.AddMinutes(5);
            user.IsOtpVerified = false;
            await _userManager.UpdateAsync(user);
            return otp;
        }
        public async Task<AuthResponseDto> VerifyOtpAsync(VerifyOtpDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid email or OTP."
                };
            }
            if (string.IsNullOrEmpty(user.LoginOtp) || user.LoginOtpExpiry == null)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "OTP not found or already used."
                };
            }
            if(user.LoginOtpExpiry < DateTime.UtcNow)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "OTP has expired."
                };
            }
            if(user.LoginOtp != dto.Otp)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid OTP."
                };
            }
            // OTP verfied
            //user.IsOtpVerified = true;
            //user.LoginOtp = null;
            //user.LoginOtpExpiry = null;
            //await _userManager.UpdateAsync(user);
            // Generate JWT
            //var roles = await _userManager.GetRolesAsync(user);
            //var claims = new List<Claim>
            //{
            //    new Claim(ClaimTypes.NameIdentifier,user.Id),
            //    new Claim(ClaimTypes.Name,user.UserName),
            //    new Claim(ClaimTypes.Email,user.Email)
            //};
            //foreach(var role in roles)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, role));
            //}
            //var key = new SymmetricSecurityKey(
            //    Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            //    );
            //var creds = new SigningCredentials(key,
            //    SecurityAlgorithms.HmacSha256);
            //var token = new JwtSecurityToken(
            //    issuer: _configuration["Jwt:Issuer"],
            //    audience: _configuration["Jwt:Audience"],
            //    claims: claims,
            //    expires: DateTime.Now.AddMinutes(
            //        Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])
            //    ),
            //    signingCredentials: creds
            //    );
            //return new AuthResponseDto
            //{
            //    IsSuccess = true,
            //    Message = "OTP login successful.",
            //    Token = new JwtSecurityTokenHandler().WriteToken(token)
            //};
            user.IsOtpVerified = true;
            user.LoginOtp = null;
            user.LoginOtpExpiry = null;

            await _userManager.UpdateAsync(user);

            return await GenerateAuthTokensAsync(user);
        }
        public async Task<AuthResponseDto> RefreshTokenAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!)
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var accessToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
                signingCredentials: credentials
            );
            var refreshToken = Guid.NewGuid().ToString("N");
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);
            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Authentication tokens generated successfully.",
                Token = new JwtSecurityTokenHandler().WriteToken(accessToken),
                RefreshToken = refreshToken
            };
        }
        public async Task<AuthResponseDto> GenerateAuthTokensAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Email,user.Email)
            };
            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);
            var accessToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])
                ),
                signingCredentials: credentials
            );
            var refreshToken = Guid.NewGuid().ToString("N");
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);
            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Authentication tokens generated successfully.",
                Token = new JwtSecurityTokenHandler().WriteToken(accessToken),
                RefreshToken = refreshToken
            };
        }
        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == dto.RefreshToken);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid refresh token."
                };
            }
            if (user.RefreshTokenExpiry == null ||
                user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Refresh token has expired."
                };
            }

            return await GenerateAuthTokensAsync(user);
        }
        public async Task<string> GenerateEmailConfirmationTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return string.Empty;
            }

            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }
    }
}
