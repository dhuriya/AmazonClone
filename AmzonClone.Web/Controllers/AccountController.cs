using AmazonClone.Shared.Responses;
using AmzonClone.Web.Models.Account;
using AmzonClone.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AmzonClone.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IApiClient _apiClient;

        public AccountController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new CustomerLoginViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(CustomerLoginViewModel model,string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response = await _apiClient.PostAsync<CustomerLoginRequest,ApiResponse<CustomerLoginResponse>>("Auth/login",
                        new CustomerLoginRequest
                        {
                            Email = model.Email,
                            Password = model.Password
                        });

                var login = response?.Data;
                if (login?.IsSuccess != true || string.IsNullOrWhiteSpace(login.Token))
                {
                    ModelState.AddModelError(string.Empty,login?.Message ?? "Login failed.");
                    ViewBag.ReturnUrl = returnUrl;
                    return View(model);
                }
                if (!login.Roles.Contains("Customer", StringComparer.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(string.Empty,"This account does not have customer access.");
                    ViewBag.ReturnUrl = returnUrl;
                    return View(model);
                }

                HttpContext.Session.SetString("CustomerAccessToken", login.Token);
                if (!string.IsNullOrWhiteSpace(login.RefreshToken))
                {
                    HttpContext.Session.SetString("CustomerRefreshToken", login.RefreshToken);
                }
                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                ModelState.AddModelError(string.Empty,"Could not reach the login service.");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View(new CustomerRegisterViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CustomerRegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                var response = await _apiClient.PostAsync<CustomerRegisterRequest,ApiResponse<CustomerRegisterResponse>>("Auth/register",
                new CustomerRegisterRequest
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = model.Password,
                    ConfirmPassword = model.ConfirmPassword
                });
                if (response?.Success == true && response.Data?.IsSuccess == true)
                {
                    TempData["RegistrationNotice"] = "Your account was created. Email verification delivery is not configured yet.";
                    return RedirectToAction(nameof(Login));
                }
                ModelState.AddModelError(string.Empty,response?.Data?.Message ?? response?.Message ?? "Could not create your account.");
            }
            catch
            {
                ModelState.AddModelError(string.Empty,"Could not reach the registration service.");
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("CustomerAccessToken");
            HttpContext.Session.Remove("CustomerRefreshToken");
            return RedirectToAction("Index", "Home");
        }
    }
}
