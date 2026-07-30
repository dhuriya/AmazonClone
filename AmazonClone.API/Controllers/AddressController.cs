using AmazonClone.Application.Common;
using AmazonClone.Application.Features.Addresses.DTOs;
using AmazonClone.Application.Features.Addresses.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AmazonClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AddressController : Controller
    {
        private readonly IAddressService _addressService;
        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateAddressDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _addressService.CreateAsync(userId, dto);
            return Ok(new ApiResponse<AddressDto>
            {
                Success = true,
                Message = "Address added successfully.",
                Data = result
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetMyAddresses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _addressService.GetMyAddressesAsync(userId);
            return Ok(new ApiResponse<List<AddressDto>>
            {
                Success = true,
                Message = result.Any()? "Address feched successfully."
                : "No address found.",
                Data = result
            });
        }
        [HttpDelete("{addressId}")]
        public async Task<IActionResult> Delete(int addressId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _addressService.DeleteAsync(userId, addressId);
            if (!result)
            {
                return BadRequest(new ApiResponse<AddressDto>
                {
                    Success = false,
                    Message = "Address not found."
                });
            }
            return Ok(new ApiResponse<AddressDto>
            {
                Success = true,
                Message = "Address deleted successfully"
            });
        }
    }
}
