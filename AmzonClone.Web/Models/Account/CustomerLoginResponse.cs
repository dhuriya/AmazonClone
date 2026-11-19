namespace AmzonClone.Web.Models.Account
{
    public class CustomerLoginResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
