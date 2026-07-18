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
    [Authorize(Roles =Roles.Admin)]
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
            return Ok(products);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            var result = await _productService.CreateAsync(dto);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        [HttpPut]
        public async Task<IActionResult> Update(UpdateProductDto dto)
        {
            var result = await _productService.UpdateAsync(dto);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return Ok("Product deleted successfully");
        }
    }
}
