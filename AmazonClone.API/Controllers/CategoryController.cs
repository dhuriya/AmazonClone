using AmazonClone.Application.Features.Categories.DTOs;
using AmazonClone.Application.Features.Categories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AmazonClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet]
        public async Task<IActionResult> Getll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            var category = await _categoryService.CreateAsync(dto);
            return Ok(category);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();
            return Ok(category);
        }
        [HttpPut]
        public async Task<IActionResult> Update(UpdateCategoryDto dto)
        {
            var category = await _categoryService.UpdateAsync(dto);
            return Ok(category);
        }
    }
}
