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

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "User registered successfully."
            };
        }
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if(user==null)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Invlid email or  password."
                };
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if(!result.Succeeded)
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
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
                signingCredentials: creds
             );
            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Login Successfull.",
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }

    }
}
