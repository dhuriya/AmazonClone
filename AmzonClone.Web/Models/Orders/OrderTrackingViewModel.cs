namespace AmzonClone.Web.Models.Orders
{
    public class OrderTrackingViewModel
    {
        public string Status { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
