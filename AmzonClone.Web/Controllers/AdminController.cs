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

    }
}
