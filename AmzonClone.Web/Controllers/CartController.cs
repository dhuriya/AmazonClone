using AmazonClone.Shared.Responses;
using AmzonClone.Web.Models.Cart;
using AmzonClone.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AmzonClone.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IApiClient _apiClient;
        private const string CustomerTokenKey = "CustomerAccessToken";

        public CartController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!HasCustomerSession())
            {
                return RedirectToAction("Login","Account",new { returnUrl = Url.Action(nameof(Index), "Cart") });
            }
            try
            {
                var response = await _apiClient.GetAsync<ApiResponse<CartViewModel>>("Cart");
                return View(response?.Data ?? new CartViewModel());
            }
            catch (HttpRequestException)
            {
                ViewBag.CartError = "Could not load your cart. Please try again.";
                return View(new CartViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddToCartViewModel model)
        {
            if (!HasCustomerSession())
            {
                var returnUrl = Url.Action("Details","Product",new { id = model.ProductId });
                return RedirectToAction("Login","Account",new { returnUrl });
            }
            if (!ModelState.IsValid)
            {
                TempData["ProductError"] = "Enter a valid quantity.";
                return RedirectToAction("Details","Product",new { id = model.ProductId });
            }
            try
            {
                var response = await _apiClient.PostAsync<AddToCartViewModel,ApiResponse<object>>("Cart", model);
                if (response?.Success == true)
                {
                    TempData["CartSuccess"] = "Item added to your cart.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ProductError"] = response?.Message ?? "Could not add the item to your cart.";
            }
            catch (HttpRequestException)
            {
                TempData["ProductError"] = "Could not add the item. Check available stock and try again.";
            }

            return RedirectToAction("Details","Product",new { id = model.ProductId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(UpdateCartQuantityViewModel model)
        {
            if (!HasCustomerSession())
            {
                return RedirectToAction("Login","Account",new { returnUrl = Url.Action(nameof(Index), "Cart") });
            }
            if (!ModelState.IsValid)
            {
                TempData["CartError"] = "Enter a valid item quantity.";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                var response = await _apiClient.PutAsync<UpdateCartQuantityViewModel,ApiResponse<object>>("Cart", model);
                TempData[response?.Success == true ? "CartSuccess" : "CartError"] =
                    response?.Message ?? "Could not update the cart item.";
            }
            catch (HttpRequestException)
            {
                TempData["CartError"] =
                    "Could not update the quantity. Check available stock.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int productId)
        {
            if (!HasCustomerSession())
            {
                return RedirectToAction("Login","Account",new { returnUrl = Url.Action(nameof(Index), "Cart") });
            }
            if (productId <= 0)
            {
                TempData["CartError"] = "Invalid product.";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                var removed = await _apiClient.DeleteAsync($"Cart/{productId}");

                TempData[removed ? "CartSuccess" : "CartError"] = removed
                    ? "Item removed from your cart."
                    : "Could not remove the item from your cart.";
            }
            catch (HttpRequestException)
            {
                TempData["CartError"] =
                    "Could not remove the item. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool HasCustomerSession()
        {
            return !string.IsNullOrWhiteSpace(
                HttpContext.Session.GetString(CustomerTokenKey));
        }
    }
}
