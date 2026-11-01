namespace AmzonClone.Web.Models.Admin
{
    public class AdminLoginResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
