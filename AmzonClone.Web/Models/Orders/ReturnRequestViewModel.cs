using System.ComponentModel.DataAnnotations;

namespace AmzonClone.Web.Models.Orders
{
    public class ReturnRequestViewModel
    {
        [Required]
        [StringLength(500, MinimumLength = 5)]
        public string Reason { get; set; } = string.Empty;
    }
}
