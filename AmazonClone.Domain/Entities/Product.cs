using AmazonClone.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; } = true;
        // Foreign Key
        public int CategoryId { get; set; }
        // Navigation Property
        public Category Category { get; set; } = null!;
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<Wishlist> Whislists { get; set; } = new List<Wishlist>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
