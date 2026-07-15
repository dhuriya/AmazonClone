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
        //-----------------------------------------------------------
        //this is for get connection string or database connection
        //-----------------------------------------------------------
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        //----------------------------------------------------------
        // This is for get all category list action method
        //----------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Getll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }
        //----------------------------------------------------------
        // This is for create new category action method
        //----------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            var category = await _categoryService.CreateAsync(dto);
            return Ok(category);
        }
        //----------------------------------------------------------
        // This is for get category by Id action method
        //----------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();
            return Ok(category);
        }
        //----------------------------------------------------------
        // This is for update category action method
        //----------------------------------------------------------
        [HttpPut]
        public async Task<IActionResult> Update(UpdateCategoryDto dto)
        {
            var category = await _categoryService.UpdateAsync(dto);
            return Ok(category);
        }
        //----------------------------------------------------------
        // This is for delete category action method
        //----------------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteAsync(id);
            if (!result)
                return NotFound();
            return Ok("Category deteled successfully.");
        }
    }
}
