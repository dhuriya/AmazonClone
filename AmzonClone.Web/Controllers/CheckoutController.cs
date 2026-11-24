using AmazonClone.Shared.Responses;
using AmzonClone.Web.Models.Address;
using AmzonClone.Web.Models.Cart;
using AmzonClone.Web.Models.Checkout;
using AmzonClone.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AmzonClone.Web.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IApiClient _apiClient;

        public CheckoutController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!HasCustomerSession())
            {
                return RedirectToAction("Login","Account",new { returnUrl = Url.Action(nameof(Index), "Checkout") });
            }

            try
            {
                var cartResponse = await _apiClient.GetAsync<ApiResponse<CartViewModel>>("Cart");
                var addressResponse = await _apiClient.GetAsync<ApiResponse<List<AddressViewModel>>>("Address");
                var cart = cartResponse?.Data ?? new CartViewModel();

                if (!cart.Items.Any())
                    return RedirectToAction("Index", "Cart");
                var addresses = addressResponse?.Data ?? new List<AddressViewModel>();
                var model = new CheckoutViewModel
                {
                    Cart = cart,
                    Addresses = addresses,
                    SelectedAddressId = addresses.FirstOrDefault(address => address.IsDefault)?.Id ?? addresses.FirstOrDefault()?.Id ?? 0
                };
                return View(model);
            }
            catch (HttpRequestException)
            {
                ViewBag.CheckoutError = "Could not load checkout details. Please try again.";
                return View(new CheckoutViewModel());
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            if (!HasCustomerSession())
            {
                return RedirectToAction("Login","Account",new { returnUrl = Url.Action(nameof(Index), "Checkout") });
            }

            if (model.SelectedAddressId <= 0)
            {
                TempData["CheckoutError"] = "Select a delivery address.";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                var request = new PlaceOrderRequest
                {
                    AddressId = model.SelectedAddressId,
                    PaymentMethod = "COD"
                };
                var response = await _apiClient.PostAsync<PlaceOrderRequest,ApiResponse<OrderConfirmationViewModel>>("Order/checkout",request);

                if (response?.Success == true && response.Data is not null)
                {
                    return RedirectToAction(nameof(Confirmation),new { id = response.Data.Id });
                }

                TempData["CheckoutError"] = response?.Message ?? "Could not place your order.";
            }
            catch (HttpRequestException)
            {
                TempData["CheckoutError"] = "Stock or cart details may have changed. Review your cart and try again.";
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            if (!HasCustomerSession())
            {
                return RedirectToAction("Login","Account",new { returnUrl = Url.Action(nameof(Confirmation), new { id }) });
            }

            if (id <= 0)
                return BadRequest();

            try
            {
                var response = await _apiClient.GetAsync<ApiResponse<OrderConfirmationViewModel>>($"Order/{id}");
                if (response?.Data is null)
                    return NotFound();
                return View(response.Data);
            }
            catch (HttpRequestException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable,"Order confirmation is temporarily unavailable.");
            }
        }
        private bool HasCustomerSession()
        {
            return !string.IsNullOrWhiteSpace(HttpContext.Session.GetString("CustomerAccessToken"));
        }
    }
}
