using AmazonClone.Shared.Responses;
using AmzonClone.Web.Models.Orders;
using AmzonClone.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AmzonClone.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IApiClient _apiClient;

        public OrdersController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!HasCustomerSession())
                return RedirectToLogin(Url.Action(nameof(Index)));
            try
            {
                var response = await _apiClient.GetAsync<ApiResponse<List<CustomerOrderViewModels>>>("Order");

                return View(response?.Data ?? new List<CustomerOrderViewModels>());
            }
            catch (HttpRequestException)
            {
                ViewBag.OrderError = "Could not load your orders. Please try again.";
                return View(new List<CustomerOrderViewModels>());
            }
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (!HasCustomerSession())
                return RedirectToLogin(Url.Action(nameof(Details), new { id }));

            if (id <= 0)
                return BadRequest();
            try
            {
                var response = await _apiClient.GetAsync<ApiResponse<CustomerOrderViewModels>>($"Order/{id}");
                return response?.Data is null ? NotFound() : View(response.Data);
            }
            catch (HttpRequestException ex)
                when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound();
            }
            catch (HttpRequestException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable,"Order details are temporarily unavailable.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Tracking(int id)
        {
            if (!HasCustomerSession())
                return RedirectToLogin(Url.Action(nameof(Tracking), new { id }));
            if (id <= 0)
                return BadRequest();
            try
            {
                var response = await _apiClient.GetAsync<ApiResponse<List<OrderTrackingViewModel>>>($"Order/{id}/tracking");
                ViewBag.OrderId = id;
                return View(response?.Data ?? new List<OrderTrackingViewModel>());
            }
            catch (HttpRequestException ex)
                when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                ViewBag.TrackingError = "Tracking information is not available yet.";
                ViewBag.OrderId = id;
                return View(new List<OrderTrackingViewModel>());
            }
            catch (HttpRequestException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable,"Tracking information is temporarily unavailable.");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            if (!HasCustomerSession())
                return RedirectToLogin(Url.Action(nameof(Index)));

            if (id <= 0)
                return BadRequest();

            try
            {
                var response = await _apiClient.PutAsync<Dictionary<string, string>,ApiResponse<object>>($"Order/{id}/cancel",new Dictionary<string, string>());
                TempData[response?.Success == true ? "OrderSuccess" : "OrderError"] = response?.Message ?? "Could not cancel this order.";
            }
            catch (HttpRequestException)
            {
                TempData["OrderError"] = "This order could not be cancelled.";
            }
            return RedirectToAction(nameof(Details), new { id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestReturn(int id,ReturnRequestViewModel model)
        {
            if (!HasCustomerSession())
                return RedirectToLogin(Url.Action(nameof(Details), new { id }));

            if (id <= 0)
                return BadRequest();
            if (!ModelState.IsValid)
            {
                TempData["OrderError"] = "Enter a return reason with at least 5 characters.";
                return RedirectToAction(nameof(Details), new { id });
            }
            try
            {
                var response = await _apiClient.PostAsync<CreateReturnRequest,ApiResponse<object>>($"Order/{id}/return",new CreateReturnRequest { Reason = model.Reason });
                TempData[response?.Success == true ? "OrderSuccess" : "OrderError"] = response?.Message ?? "Could not submit the return request.";
            }
            catch (HttpRequestException)
            {
                TempData["OrderError"] = "Could not submit the return request.";
            }
            return RedirectToAction(nameof(Details), new { id });
        }
        [HttpGet]
        public async Task<IActionResult> Invoice(int id)
        {
            if (!HasCustomerSession())
                return RedirectToLogin(Url.Action(nameof(Invoice), new { id }));
            if (id <= 0)
                return BadRequest();

            try
            {
                var response = await _apiClient.GetAsync<ApiResponse<CustomerOrderViewModels>>($"Order/{id}");
                return response?.Data is null ? NotFound() : View(response.Data);
            }
            catch (HttpRequestException ex)
                when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound();
            }
            catch (HttpRequestException)
            {
                return StatusCode( StatusCodes.Status503ServiceUnavailable, "The invoice is temporarily unavailable.");
            }
        }
        private bool HasCustomerSession()
        {
            return !string.IsNullOrWhiteSpace(HttpContext.Session.GetString("CustomerAccessToken"));
        }
        private IActionResult RedirectToLogin(string? returnUrl)
        {
            return RedirectToAction( "Login","Account",new { returnUrl });
        }
    }
}
