using AmazonClone.Application.Common;
using AmazonClone.Application.Features.Addresses.DTOs;
using AmazonClone.Application.Features.Addresses.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace AmazonClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [SwaggerTag("Address Management")]
    public class AddressController : Controller
    {
        private readonly IAddressService _addressService;
        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }
        [HttpPost]
        [SwaggerOperation(
            Summary = "Add a new address",
            Description = "Adds a new address for the authenticated user."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Address added successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
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
        [SwaggerOperation(
            Summary = "Get my addresses",
            Description = "Returns all addresses of the authenticated user."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Addresses retrieved successfully.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
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
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete address",
            Description = "Deletes an address of the authenticated user."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Address deleted successfully.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Address not found.")]
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
        [HttpPut]
        [SwaggerOperation(
            Summary = "Update address",
            Description = "Updates an existing address for the authenticated user."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Address updated successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Address not found.")]
        public async Task<IActionResult> Update(UpdateAddressDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await _addressService.UpdateAsync(userId, dto);

            return Ok(new ApiResponse<AddressDto>
            {
                Success = true,
                Message = "Address updated successfully.",
                Data = result
            });
        }
    }
}
