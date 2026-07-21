using AmazonClone.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AmazonClone.Application.Features.Categories.Interfaces;
using AmazonClone.Persistence.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmazonClone.Application.Features.Products.Interfaces;
using AmazonClone.Application.Features.Auth.Interfaces;
using AmazonClone.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using AmazonClone.Application.Features.Cart.Interfaces;
using AmazonClone.Application.Features.Orders.Interfaces;
using AmazonClone.Application.Features.Wishlist.Interfaces;
using AmazonClone.Application.Features.Addresses.Interfaces;

namespace AmazonClone.Persistence.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<IAddressService, AddressService>();
            return services;
        }
    }
}
