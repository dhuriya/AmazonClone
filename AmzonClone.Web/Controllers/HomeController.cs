using AmzonClone.Web.Models;
using AmzonClone.Web.Models.Home;
using AmzonClone.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AmzonClone.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IApiClient _apiClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IApiClient apiClient, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(string? search, int? categoryId)
        {
            var model = new HomeViewModel { SearchTerm = search?.Trim(), SelectedCategoryId = categoryId };
            try
            {
                var response = await _apiClient.GetAsync<ApiResponse<List<CategoryViewModel>>>("Category");
                model.Categories = response?.Data?.ToList() ?? new List<CategoryViewModel>();

                var featuredResponse = await _apiClient.GetAsync<ApiResponse<List<ProductViewModel>>>("Product");
                model.FeaturedProducts = featuredResponse?.Data?.ToList() ?? new List<ProductViewModel>();
                if (categoryId is > 0)
                    model.FeaturedProducts = model.FeaturedProducts.Where(p => p.CategoryId == categoryId).ToList();
                if (!string.IsNullOrWhiteSpace(model.SearchTerm))
                    model.FeaturedProducts = model.FeaturedProducts.Where(p =>
                        p.Name.Contains(model.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (p.CategoryName?.Contains(model.SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (p.Description?.Contains(model.SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                model.ErrorMessage = "Unable to fetch data from the API. Please try again later.";
            }
            ViewBag.SearchTerm = model.SearchTerm;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ProductImage(string? fileName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return BadRequest();

            var path = fileName.Replace('\\', '/');
            if (Uri.TryCreate(fileName, UriKind.Absolute, out var imageUri))
                path = imageUri.AbsolutePath;

            var safeFileName = Path.GetFileName(path);
            var extension = Path.GetExtension(safeFileName).ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(safeFileName) ||
                !new[] { ".jpg", ".jpeg", ".png", ".webp" }.Contains(extension))
                return BadRequest();

            if (!Uri.TryCreate(_configuration["ApiSettings:BaseUrl"], UriKind.Absolute, out var apiBaseUrl))
                return StatusCode(StatusCodes.Status500InternalServerError);

            var imageUriToFetch = new Uri(apiBaseUrl, $"../uploads/{Uri.EscapeDataString(safeFileName)}");
            using var response = await _httpClientFactory.CreateClient().GetAsync(imageUriToFetch, cancellationToken);
            if (!response.IsSuccessStatusCode) return NotFound();

            var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            return File(bytes, contentType);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
