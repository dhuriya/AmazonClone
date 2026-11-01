using System.ComponentModel.DataAnnotations;

namespace AmzonClone.Web.Models.Admin
{
    public class AdminLoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
