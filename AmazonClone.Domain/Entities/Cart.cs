using AmazonClone.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Domain.Entities
{
    public class Cart : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null;
        public ICollection<CartItem> CartItem { get; set; } = new List<CartItem>();
    }
}
