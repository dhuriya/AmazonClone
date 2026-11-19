using AmazonClone.Shared.Responses;
using AmzonClone.Web.Models.Admin;
using AmzonClone.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net.Http.Headers;

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
            HttpContext.Session.Remove("AdminAccessToken");
            HttpContext.Session.Remove("AdminRefreshToken");
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
                TempData["CategoryError"] = "Enter a category name with no more than 100 characters.";
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
                    result?.Message ?? "Could not create the category.";
            }
            catch
            {
                TempData["CategoryError"] =
                    "Could not create the category. Please check the API connection.";
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
                return StatusCode(502, new { message = "Could not load the category details." });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCategory(int id,string name,string? description, bool isActive = true)
        {
            if(id <= 0 || string.IsNullOrWhiteSpace(name) || name.Trim().Length > 100)
            {
                TempData["CategoryError"] = "Enter valid category details and try again.";
                return RedirectToAction(nameof(Categories));
            }
            try
            {
                var request = new UpdateCategoryRequest
                {
                    Id = id,
                    Name = name.Trim(),
                    Description = description,
                    IsActive = isActive
                };
                var result = await _apiClient.PutAsync<UpdateCategoryRequest, ApiResponse<CategoryAdminViewModel>>("Category", request);
                TempData[result?.Success == true ? "CategorySuccess" : "CategoryError"] =
                result?.Message ?? "Could not update the category.";
            }
            catch
            {
                TempData["CategoryError"] = "Could not update the category. Please check the API connection.";
            }
            return RedirectToAction(nameof(Categories));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if(id<=0)
            {
                TempData["CategoryError"] = "Invalid category.";
                return RedirectToAction(nameof(Categories));
            }
            try
            {
                var deleted = await _apiClient.DeleteAsync($"Category/{id}");
                TempData[deleted ? "CategorySuccess" : "CategoryError"] = deleted ? "Category deleted successfully." : "Could not delete the category.";
            }
            catch
            {
                TempData["CategoryError"] = "Could not delete the category. Please check the API connection.";
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
        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            var model = new ProductCreateViewModel();
            await LoadCategoriesAsync(model);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(ProductCreateViewModel model)
        {
            if (model.Image is not null && model.Image.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError(nameof(model.Image),"Image size must not exceed 2 MB.");
            }
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(model);
                return View(model);
            }
            try
            {
                using var content = new MultipartFormDataContent();
                content.Add(new StringContent(model.Name.Trim()), "Name");
                content.Add(new StringContent(model.Description.Trim()), "Description");
                content.Add(new StringContent(model.Price.ToString(CultureInfo.InvariantCulture)), "Price");
                content.Add(new StringContent(model.Stock.ToString(CultureInfo.InvariantCulture)), "Stock");
                content.Add(new StringContent(model.CategoryId.ToString(CultureInfo.InvariantCulture)), "CategoryId");
                content.Add(new StringContent(model.IsFeatured.ToString()), "IsFeatured");
                if (model.Image is { Length: > 0 })
                {
                    var imageContent = new StreamContent(model.Image.OpenReadStream());
                    if (MediaTypeHeaderValue.TryParse(model.Image.ContentType, out var contentType))
                    {
                        imageContent.Headers.ContentType = contentType;
                    }
                    content.Add(imageContent,"Image",Path.GetFileName(model.Image.FileName));
                }
                var result = await _apiClient.PostMultipartAsync<ApiResponse<ProductAdminViewModel>>("Product", content);
                if (result?.Success == true)
                {
                    TempData["ProductSuccess"] = "Product created successfully.";
                    return RedirectToAction(nameof(Products));
                }

                ModelState.AddModelError(string.Empty,result?.Message ?? "Could not create the product.");
            }
            catch
            {
                ModelState.AddModelError(string.Empty,"Could not create the product. Check the API connection and image format.");
            }
            await LoadCategoriesAsync(model);
            return View(model);
        }
        private async Task LoadCategoriesAsync(ProductCreateViewModel model)
        {
            var response = await _apiClient.GetAsync<ApiResponse<List<CategoryAdminViewModel>>>("Category");
            model.Categories = response?.Data ?? new List<CategoryAdminViewModel>();
        }
        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var response =
                await _apiClient.GetAsync<ApiResponse<ProductAdminViewModel>>(
                    $"Product/{id}");

            if (response?.Data is null)
                return NotFound();

            var product = response.Data;

            var model = new ProductCreateViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                IsFeatured = product.IsFeatured,
                IsActive = product.IsActive,
                ExistingImageUrl = product.ImageUrl
            };

            await LoadCategoriesAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(ProductCreateViewModel model)
        {
            if (model.Id <= 0)
                return NotFound();
            if (model.Image is not null && model.Image.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError(nameof(model.Image), "Image size must not exceed 2 MB.");
            }
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(model);
                return View(model);
            }
            try
            {
                using var content = new MultipartFormDataContent();
                content.Add(new StringContent(model.Id.ToString()), "Id");
                content.Add(new StringContent(model.Name.Trim()), "Name");
                content.Add(new StringContent(model.Description.Trim()), "Description");
                content.Add(new StringContent(model.Price.ToString(CultureInfo.InvariantCulture)), "Price");
                content.Add(new StringContent(model.Stock.ToString(CultureInfo.InvariantCulture)), "Stock");
                content.Add(new StringContent(model.CategoryId.ToString(CultureInfo.InvariantCulture)), "CategoryId");
                content.Add(new StringContent(model.IsFeatured.ToString()), "IsFeatured");
                content.Add(new StringContent(model.IsActive.ToString()), "IsActive");

                if (model.Image is { Length: > 0 })
                {
                    var imageContent = new StreamContent(model.Image.OpenReadStream());
                    if (MediaTypeHeaderValue.TryParse(model.Image.ContentType, out var contentType))
                    {
                        imageContent.Headers.ContentType = contentType;
                    }
                    content.Add(imageContent, "Image", Path.GetFileName(model.Image.FileName));
                }
                var result = await _apiClient.PutMultipartAsync<ApiResponse<ProductAdminViewModel>>("Product", content);
                if (result?.Success == true)
                {
                    TempData["ProductSuccess"] = "Product updated successfully.";
                    return RedirectToAction(nameof(Products));
                }
                ModelState.AddModelError(string.Empty, result?.Message ?? "Could not update the product.");
            }
            catch
            {
                ModelState.AddModelError(
                    string.Empty, "Could not update the product. Please try again.");
            }

            await LoadCategoriesAsync(model);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id <= 0)
            {
                TempData["ProductError"] = "Invalid product.";
                return RedirectToAction(nameof(Products));
            }
            try
            {
                var deleted = await _apiClient.DeleteAsync($"Product/{id}");
                TempData[deleted ? "ProductSuccess" : "ProductError"] = deleted
                        ? "Product deleted successfully."
                        : "Could not delete the product. Check your admin access.";
            }
            catch
            {
                TempData["ProductError"] =
                    "Could not delete the product. Please check the API connection.";
            }
            return RedirectToAction(nameof(Products));
        }
        public async Task<IActionResult> Orders()
        {
            try
            {
                var response = await _apiClient.GetAsync<ApiResponse<List<OrderAdminViewModel>>>("Order/admin");
                return View(response?.Data ?? new List<OrderAdminViewModel>());
            }
            catch
            {
                ViewBag.OrderApiError = "Could not load orders. Please check your admin login and API connection.";
                return View(new List<OrderAdminViewModel>());
            }
        }
    }
}
