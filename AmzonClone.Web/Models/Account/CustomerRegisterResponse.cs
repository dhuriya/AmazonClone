namespace AmzonClone.Web.Models.Account
{
    public class CustomerRegisterResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string? EmailVerification { get; set; }
    }
}
