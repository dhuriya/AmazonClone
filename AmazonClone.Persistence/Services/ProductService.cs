using AmazonClone.Application.Features.Products.DTOs;
using AmazonClone.Application.Features.Products.Interfaces;
using AmazonClone.Domain.Entities;
using AmazonClone.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Persistence.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductDto>> GetAllAsync()
        {
            return await _context.Products.Where(p =>!p.IsDeleted).
                Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price =p.Price,
                    Stock = p.Stock,
                    ImageUrl = p.ImageUrl,
                    IsFeatured = p.IsFeatured,
                    IsActive = p.IsActive,
                    CategoryId = p.CategoryId
                }).ToListAsync();
        }
        //----------------------
        // Create Project
        //----------------------
        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                ImageUrl = dto.ImageUrl,
                IsFeatured = dto.IsFeatured,
                IsActive = true,
                CategoryId = dto.CategoryId
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl,
                IsFeatured = product.IsFeatured,
                IsActive = product.IsActive,
                CategoryId = product.CategoryId
            };
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            return await _context.Products.Where(p => p.Id == id && !p.IsDeleted)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    ImageUrl = p.ImageUrl,
                    IsFeatured = p.IsFeatured,
                    IsActive = p.IsActive,
                    CategoryId = p.CategoryId
                }).FirstOrDefaultAsync();
        }

        public async Task<ProductDto> UpdateAsync(UpdateProductDto dto)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == dto.Id && !p.IsDeleted);
            if(product == null)
                throw new Exception("Product not found");
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.ImageUrl = dto.ImageUrl;
            product.IsFeatured = dto.IsFeatured;
            product.IsActive = dto.IsActive;
            product.CategoryId = dto.CategoryId;
            await _context.SaveChangesAsync();
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl,
                IsFeatured = product.IsFeatured,
                IsActive = product.IsActive,
                CategoryId = product.CategoryId
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
            if (product == null)
                return false;
            product.IsDeleted = true;
            product.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
