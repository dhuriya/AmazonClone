using System.ComponentModel.DataAnnotations;

namespace AmzonClone.Web.Models.Admin
{
    public class UpdateOrderStatusRequest
    {
        [Range(1, int.MaxValue)]
        public int OrderId { get; set; }

        [Range(1, 14)]
        public int Status { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        [StringLength(150)]
        public string? Location { get; set; }
    }
}
