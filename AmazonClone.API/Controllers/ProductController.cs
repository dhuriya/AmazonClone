using AmazonClone.API.Models;
using AmazonClone.Application.Common;
using AmazonClone.Application.Features.Products.DTOs;
using AmazonClone.Application.Features.Products.Interfaces;
using AmazonClone.Shared.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AmazonClone.API.Controllers
{
    [SwaggerTag("Product Management")]
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
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Get all products",
            Description = "Returns all available products."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Products retrieved successfully.")]
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
        [SwaggerOperation(
            Summary ="Create a new product",
            Description = "Creates a new product. Only Admin users are allowed")]
        [SwaggerResponse(StatusCodes.Status200OK, "Product created successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest,"Invalid request")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized,"Unauthrorized")]
        [SwaggerResponse(StatusCodes.Status403Forbidden,"Forbidden")]
        public async Task<IActionResult> Create([FromForm] CreateProductRequest request)
        {
            string imagePath = string.Empty;
            if(request.Image != null && request.Image.Length>0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if(!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var validationResult = ValidateImage(request.Image);

                if (validationResult != null)
                    return validationResult;
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Image.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await request.Image.CopyToAsync(stream);
                imagePath = $"uploads/{fileName}";
            }
            var dto = new CreateProductDto
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                CategoryId = request.CategoryId,
                IsFeatured = request.IsFeatured,
                ImageUrl = imagePath
            };
            var result = await _productService.CreateAsync(dto);
            return Ok(new ApiResponse<ProductDto>
            {
                Success = true,
                Message = "Product created successfully.",
                Data = result
            });
        }
        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Get product by id",
            Description = "Returns product details by product id."
        )]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
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
        [SwaggerOperation(
            Summary = "Update product",
            Description = "Updates an existing product. Admin only."
        )]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        //[Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Update([FromForm] UpdateProductRequest request)
        {
            var existingProduct = await _productService.GetByIdAsync(request.Id);
            if(existingProduct==null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Product not found."
                });
            }
            string imagePath = existingProduct.ImageUrl;
            if (request.Image != null && request.Image.Length > 0)
            {
                // Validation baad me add karenge
                if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                {
                    var oldImage = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot",existingProduct.ImageUrl);
                    if (System.IO.File.Exists(oldImage))
                    { 
                        System.IO.File.Delete(oldImage);
                    }
                }
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot","uploads");
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Image.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                
                var validationResult = ValidateImage(request.Image);
                if (validationResult != null)
                    return validationResult;
                await request.Image.CopyToAsync(stream);
                imagePath = $"uploads/{fileName}";
            }
            var dto = new UpdateProductDto
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                ImageUrl = imagePath,
                IsFeatured = request.IsFeatured,
                IsActive = request.IsActive,
                CategoryId = request.CategoryId
            };
            var result = await _productService.UpdateAsync(dto);
            return Ok(new ApiResponse<ProductDto>
            {
                Success = true,
                Message = "Product updated successfully",
                Data = result
            });
        }
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete product",
            Description = "Soft deletes a product. Admin only."
        )]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if(product !=null && !string.IsNullOrEmpty(product.ImageUrl))
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot", product.ImageUrl);
                if(System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
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
        private IActionResult? ValidateImage(IFormFile? image)
        {
            if (image == null)
                return null;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Only JPG, JPEG, PNG and WEBP images are allowed."
                });
            }

            if (image.Length > 2 * 1024 * 1024)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Image size must not exceed 2 MB."
                });
            }
            if (image.Length == 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Image file is empty."
                });
            }
            return null;
        }
    }
}
