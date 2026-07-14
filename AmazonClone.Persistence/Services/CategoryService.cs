using AmazonClone.Application.Features.Categories.Interfaces;
using AmazonClone.Application.Features.Categories.DTOs;
using AmazonClone.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmazonClone.Domain.Entities;

namespace AmazonClone.Persistence.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<CategoryDto>> GetAllAsync()
        {
            return await _context.Categories.
                Where(c=>!c.IsDeleted).
                Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    IsActive = c.IsActive
                }).ToListAsync();
        }
        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _context.Categories.
                Where(c => !c.IsDeleted && c.Id ==id).
                Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    IsActive = c.IsActive
                }).FirstOrDefaultAsync();
            return category;
        }
        public async Task<CategoryDto?> CreateAsync(CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                IsActive = true
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive
            };
        }
        public async Task<CategoryDto> UpdateAsync(UpdateCategoryDto dto)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == dto.Id && !c.IsDeleted);
            if(category == null)
                throw new Exception("Category not found");
            category.Name = dto.Name;
            category.Description = dto.Description;
            category.IsActive = dto.IsActive;
            await _context.SaveChangesAsync();
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive
            };
        }
        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
