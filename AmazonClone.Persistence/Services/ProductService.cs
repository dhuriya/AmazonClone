using AmazonClone.Application.Features.Products.DTOs;
using AmazonClone.Application.Features.Products.Interfaces;
using AmazonClone.Persistence.Context;
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

        public Task<List<ProductDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ProductDto?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ProductDto> UpdateAsync(UpdateProductDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
