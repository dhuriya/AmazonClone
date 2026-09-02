using AmazonClone.Application.Features.Users.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Application.Features.Users.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDto?> GetProfileAsync(string userId);
        Task<bool> UpdateProfileAsync(string userId, UpdateProfileDto dto);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto dto);
        Task<bool> DeleteAccountAsync(string userId);
    }
}
