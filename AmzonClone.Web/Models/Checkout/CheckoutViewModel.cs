using AmzonClone.Web.Models.Address;
using AmzonClone.Web.Models.Cart;
using System.ComponentModel.DataAnnotations;

namespace AmzonClone.Web.Models.Checkout
{
    public class CheckoutViewModel
    {
        public CartViewModel Cart { get; set; } = new();
        public List<AddressViewModel> Addresses { get; set; } = new();

        [Range(1, int.MaxValue, ErrorMessage = "Select a delivery address.")]
        public int SelectedAddressId { get; set; }

        public string PaymentMethod { get; set; } = "COD";
    }
}
