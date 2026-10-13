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

        public HomeController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel();
            try
            {
                var response = await _apiClient.GetAsync<ApiResponse<List<CategoryViewModel>>>("Category");
                model.Categories = response?.Data?.ToList() ?? new List<CategoryViewModel>();

                var featuredResponse = await _apiClient.GetAsync<ApiResponse<List<ProductViewModel>>>("Product");
                model.FeaturedProducts = featuredResponse?.Data?.ToList() ?? new List<ProductViewModel>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                model.ErrorMessage = "Unable to fetch data from the API. Please try again later.";
            }
            return View(model);
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
