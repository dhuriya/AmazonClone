using AmazonClone.Application.Features.Addresses.DTOs;
using AmazonClone.Application.Features.Addresses.Interfaces;
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
    public class AddressService : IAddressService
    {
        private readonly ApplicationDbContext _context;
        public AddressService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<AddressDto>> GetMyAddressesAsync(string userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId && !a.IsDeleted)
                .Select(a => new AddressDto
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    PhoneNumber = a.PhoneNumber,
                    AddressLine1 = a.AddressLine1,
                    AddressLine2 = a.AddressLine2,
                    City = a.City,
                    State = a.State,
                    PostalCode = a.PostalCode,
                    Country = a.Country,
                    IsDefault = a.IsDefault
                }).ToListAsync();
        }
        public async Task<AddressDto> CreateAsync(string userId, CreateAddressDto dto)
        {
            if(dto.IsDefault)
            {
                var oldAddresses = await _context.Addresses
                    .Where(a => a.UserId == userId)
                    .ToListAsync();
                foreach (var address in oldAddresses)
                {
                    address.IsDefault = false;
                }
            }
            var addressEntity = new Address
            {
                UserId = userId,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                AddressLine1 = dto.AddressLine1,
                AddressLine2 = dto.AddressLine2,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                IsDefault = dto.IsDefault
            };
            _context.Addresses.Add(addressEntity);
            await _context.SaveChangesAsync();
            return new AddressDto
            {
                Id = addressEntity.Id,
                FullName = addressEntity.FullName,
                PhoneNumber = addressEntity.PhoneNumber,
                AddressLine1 = addressEntity.AddressLine1,
                AddressLine2 = addressEntity.AddressLine2,
                City = addressEntity.City,
                State = addressEntity.State,
                PostalCode = addressEntity.PostalCode,
                Country = addressEntity.Country,
                IsDefault = addressEntity.IsDefault
            };
        }
        public async Task<bool> DeleteAsync(string userId, int addressId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);
            if (address == null)
                return false;
            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
