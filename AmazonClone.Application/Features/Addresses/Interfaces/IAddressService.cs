using AmazonClone.Application.Features.Addresses.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Application.Features.Addresses.Interfaces
{
    public interface IAddressService
    {
        Task<List<AddressDto>> GetMyAddressesAsync(string userId);
        Task<AddressDto> CreateAsync(string userId, CreateAddressDto dto);
        Task<bool> DeleteAsync(string userId, int addressId);
    }
}
