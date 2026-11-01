using AmazonClone.Shared.Responses;
using AmzonClone.Web.Models.Admin;
using AmzonClone.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AmzonClone.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IApiClient _apiClient;
        public AdminController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View(new AdminLoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response =
                    await _apiClient.PostAsync<AdminLoginRequest, ApiResponse<AdminLoginResponse>>(
                        "Auth/login",
                        new AdminLoginRequest
                        {
                            Email = model.Email,
                            Password = model.Password
                        });

                var login = response?.Data;

                if (login?.IsSuccess != true || string.IsNullOrWhiteSpace(login.Token))
                {
                    ModelState.AddModelError(string.Empty, login?.Message ?? "Login failed.");
                    return View(model);
                }

                if (!login.Roles.Contains("Admin", StringComparer.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(string.Empty, "This account does not have admin access.");
                    return View(model);
                }

                HttpContext.Session.SetString("AdminAccessToken", login.Token);

                if (!string.IsNullOrWhiteSpace(login.RefreshToken))
                    HttpContext.Session.SetString("AdminRefreshToken", login.RefreshToken);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Could not reach the login service.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }
        public async Task<IActionResult> Categories()
        {
            try
            {
                var response = await _apiClient.GetAsync<ApiResponse<List<CategoryAdminViewModel>>>("Category");
                return View(response?.Data ?? new List<CategoryAdminViewModel>());
            }
            catch(Exception ex)
            {
                ViewBag.ApiError = ex.Message;
                return View(new List<CategoryAdminViewModel>());
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 100)
            {
                TempData["CategoryError"] = "Category name required hai aur 100 characters se chhota hona chahiye.";
                return RedirectToAction(nameof(Categories));
            }
            try
            {
                var request = new CreateCategoryRequest
                {
                    Name = name.Trim(),
                    Description = description
                };

                var result =
                    await _apiClient.PostAsync<CreateCategoryRequest, ApiResponse<CategoryAdminViewModel>>(
                        "Category", request);

                TempData[result?.Success == true ? "CategorySuccess" : "CategoryError"] =
                    result?.Message ?? "Category create nahi ho saki.";
            }
            catch
            {
                TempData["CategoryError"] =
                    "Category create nahi ho saki. API connection check karein.";
            }

            return RedirectToAction(nameof(Categories));
        }
        [HttpGet]
        public async Task<IActionResult> GetCategory(int id)
        {
            try
            {
                var response = await _apiClient.GetAsync<ApiResponse<CategoryAdminViewModel>>($"Category/{id}");
                if(response?.Data == null)
                {
                    return NotFound();
                }
                return Json(response.Data);
            }catch
            {
                return StatusCode(502, new { message = "Category is not found" });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCategory(int id,string name,string? description, bool isActive = true)
        {
            if(id <= 0 || string.IsNullOrWhiteSpace(name) || name.Trim().Length > 10)
            {
                TempData["CategoryError"] = "Category details check karke dobara try karein.";
                return RedirectToAction(nameof(Categories));
            }
            try
            {
                var request = new UpdateCategoryRequest
                {
                    Id = id,
                    Name = name,
                    Description = description,
                    IsActive = isActive
                };
                var result = await _apiClient.PutAsync<UpdateCategoryRequest, ApiResponse<CategoryAdminViewModel>>("Category", request);
                TempData[result?.Success == true ? "CategorySuccess" : "CategoryError"] =
                result?.Message ?? "Category update nahi ho saki.";
            }
            catch
            {
                TempData["CategoryError"] = "Category update nahi ho saki. API connection check karein.";
            }
            return RedirectToAction(nameof(Categories));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if(id<0)
            {
                TempData["CategoryError"] = "Invalid Category";
                return RedirectToAction(nameof(Categories));
            }
            try
            {
                var deleted = await _apiClient.DeleteAsync($"Category/{id}");
                TempData[deleted ? "CategorySuccess" : "CategoryError"] = deleted ? "Category deleted Successfully" : "Could not delete the category";
            }
            catch
            {
                TempData["CategoryError"] ="Could not delete the category. Please check the API connection.";
            }
            return RedirectToAction(nameof(Categories));
        }
        public async Task<IActionResult> Products()
        {
            try
            {
                var response = await _apiClient.GetAsync<ApiResponse<List<ProductAdminViewModel>>>("Product");
                return View(response?.Data ?? new List<ProductAdminViewModel>());
            }
            catch
            {
                ViewBag.ProductApiError =
                "Could not load products. Please check the API connection.";

                return View(new List<ProductAdminViewModel>());
            }
        }

    }
}
