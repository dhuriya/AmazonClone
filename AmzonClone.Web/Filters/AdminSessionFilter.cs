using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AmzonClone.Web.Filters
{
    public class AdminSessionFilter : IAsyncAuthorizationFilter
    {
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var controller = context.RouteData.Values["controller"]?.ToString();
            var action = context.RouteData.Values["action"]?.ToString();

            var isAdminController = string.Equals(controller, "Admin", StringComparison.OrdinalIgnoreCase);
            var isLoginAction = string.Equals(action, "Login", StringComparison.OrdinalIgnoreCase);
            var token = context.HttpContext.Session.GetString("AdminAccessToken");
            if (isAdminController && !isLoginAction && string.IsNullOrWhiteSpace(token))
            {
                context.Result = new RedirectToActionResult("Login", "Admin", null);
            }
            return Task.CompletedTask;
        }
    }
}
