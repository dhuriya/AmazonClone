using AmazonClone.Application.Common;
using AmazonClone.Application.Features.Categories.DTOs;
using AmazonClone.Application.Features.Categories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AmazonClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Category Management")]
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
        [SwaggerOperation(
            Summary = "Get all categories",
            Description = "Returns all available categories."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Categories retrieved successfully.")]
        public async Task<IActionResult> Getll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<CategoryDto>>
            {
                Success = true,
                Message = "Categories fetched successfully",
                Data = categories
            });
        }
        //----------------------------------------------------------
        // This is for create new category action method
        //----------------------------------------------------------
        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new category",
            Description = "Creates a new category. Only Admin users are allowed."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Category created successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.")]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            var category = await _categoryService.CreateAsync(dto);
            return Ok(new ApiResponse<CategoryDto>
            {
                Success = true,
                Message = "Category created successfully.",
                Data = category
            });
        }
        //----------------------------------------------------------
        // This is for get category by Id action method
        //----------------------------------------------------------
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get category by id",
            Description = "Returns category details for the specified id."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Category retrieved successfully.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Category not found.")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Category not found."
                });
            }
            return Ok(new ApiResponse<CategoryDto>
            {
                Success = true,
                Message = "Category fetched successfully.",
                Data = category
            });
        }
        //----------------------------------------------------------
        // This is for update category action method
        //----------------------------------------------------------
        [HttpPut]
        [SwaggerOperation(
            Summary = "Update category",
            Description = "Updates an existing category. Only Admin users are allowed."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Category updated successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Category not found.")]
        public async Task<IActionResult> Update(UpdateCategoryDto dto)
        {
            var category = await _categoryService.UpdateAsync(dto);
            return Ok(new ApiResponse<CategoryDto>
            {
                Success = true,
                Message = "Category updated successfully.",
                Data = category
            });
        }
        //----------------------------------------------------------
        // This is for delete category action method
        //----------------------------------------------------------
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete category",
            Description = "Soft deletes a category. Only Admin users are allowed."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Category deleted successfully.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Category not found.")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Category not found."
                });
            }
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Category deleted successfully."
            });
        }
    }
}
