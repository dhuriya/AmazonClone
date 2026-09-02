using AmazonClone.Application.Features.Users.DTOs;
using AmazonClone.Application.Features.Users.Interfaces;
using AmazonClone.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Persistence.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<UserProfileDto?> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return null;
            }
            return new UserProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber
            };
        }
        public async Task<bool> UpdateProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return false;
            }
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.PhoneNumber = dto.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            if(dto.NewPassword != dto.ConfirmPassword)
            {
                return false;
            }
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return false;
            }
            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword,
                dto.NewPassword);
            return result.Succeeded;
        }
        public async Task<bool> DeleteAccountAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return false;
            }
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
}
