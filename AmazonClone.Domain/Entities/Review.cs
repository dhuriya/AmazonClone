using AmazonClone.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Domain.Entities
{
    public class Review : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
