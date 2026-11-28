using AmzonClone.Web.Models.Home;
using AmzonClone.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AmzonClone.Web.Controllers
{
    public class ProductController : Controller
    {
        private const string CustomerTokenKey = "CustomerAccessToken";
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
                var product = response.Data;
                try
                {
                    var reviewsResponse = await _apiClient.GetAsync<ApiResponse<List<ProductReviewViewModel>>>($"Review/{id}");
                    product.Reviews = reviewsResponse?.Data ?? new List<ProductReviewViewModel>();
                }
                catch (HttpRequestException)
                {
                    product.ReviewsError = "Reviews are temporarily unavailable.";
                }
                return View(product);
            }
            catch (HttpRequestException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable,"Product details are temporarily unavailable.");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(SubmitProductReviewViewModel model)
        {
            if (!HasCustomerSession())
            {
                var returnUrl = Url.Action(nameof(Details),"Product",new { id = model.ProductId });
                return RedirectToAction("Login", "Account", new { returnUrl });
            }
            if (!ModelState.IsValid)
            {
                TempData["ReviewError"] = string.Join(" ", ModelState.Values.SelectMany(value => value.Errors)
                        .Select(error => error.ErrorMessage).Where(message => !string.IsNullOrWhiteSpace(message)));

                return RedirectToAction(nameof(Details), new { id = model.ProductId });
            }
            try
            {
                var response = await _apiClient.PostAsync<SubmitProductReviewViewModel,ApiResponse<ProductReviewViewModel>>("Review",model);
                if (response?.Success == true)
                {
                    TempData["ReviewSuccess"] = "Your review was submitted successfully.";
                }
                else
                {
                    TempData["ReviewError"] = response?.Message ?? "Could not submit your review.";
                }
            }
            catch (HttpRequestException)
            {
                TempData["ReviewError"] = "Could not submit your review. You can review a product after its order is delivered.";
            }
            return RedirectToAction(nameof(Details), new { id = model.ProductId });
        }
        private bool HasCustomerSession()
        {
            return !string.IsNullOrWhiteSpace(
                HttpContext.Session.GetString(CustomerTokenKey));
        }
    }
}
