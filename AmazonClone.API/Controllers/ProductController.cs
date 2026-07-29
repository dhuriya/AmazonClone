using AmazonClone.Application.Common;
using AmazonClone.Application.Features.Products.DTOs;
using AmazonClone.Application.Features.Products.Interfaces;
using AmazonClone.Shared.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmazonClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles =Roles.Admin)]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<ProductDto>>
            {
                Success = true,
                Message = "Products fetched successfully",
                Data = products
            });
        }
        [HttpPost]
        //[Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            var result = await _productService.CreateAsync(dto);
            return Ok(new ApiResponse<ProductDto>
            {
                Success = true,
                Message = "Product created successfully.",
                Data = result
            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Product not found."
                });
            }
            return Ok(new ApiResponse<ProductDto>
            {
                Success = true,
                Message = "Product fetched successfully.",
                Data = product
            });
        }
        [HttpPut]
        //[Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Update(UpdateProductDto dto)
        {
            var result = await _productService.UpdateAsync(dto);
            return Ok(new ApiResponse<ProductDto>
            {
                Success = true,
                Message = "Product updated successfully",
                Data = result
            });
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Product not found."
                });
            }
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Product deleted successfully."
            });
        }
        //[AllowAnonymous]
        [HttpGet("filter")]
        public async Task<IActionResult> Filter([FromQuery] ProductQueryParameters query)
        {
            var result = await _productService.GetFiltereProductsAsync(query);
            return Ok(result);
        }
    }
}
