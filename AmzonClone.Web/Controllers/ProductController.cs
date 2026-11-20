using AmzonClone.Web.Models.Home;
using AmzonClone.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AmzonClone.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IApiClient _apiClient;

        public ProductController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return BadRequest();

            try
            {
                var response = await _apiClient.GetAsync<ApiResponse<ProductViewModel>>($"Product/{id}");
                if (response?.Data is null)
                    return NotFound();
                return View(response.Data);
            }
            catch (HttpRequestException)
            {
                return StatusCode( StatusCodes.Status503ServiceUnavailable,"Product details are temporarily unavailable.");
            }
        }
    }
}
