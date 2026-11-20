using System.ComponentModel.DataAnnotations;

namespace AmzonClone.Web.Models.Cart
{
    public class AddToCartViewModel
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(1, 99)]
        public int Quantity { get; set; } = 1;
    }
}
