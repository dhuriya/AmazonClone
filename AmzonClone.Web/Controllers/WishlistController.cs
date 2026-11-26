using AmazonClone.Shared.Responses;
using AmzonClone.Web.Models.Wishlist;
using AmzonClone.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AmzonClone.Web.Controllers
{
    public class WishlistController : Controller
    {
        private const string CustomerTokenKey = "CustomerAccessToken";
        private readonly IApiClient _apiClient;

        public WishlistController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!HasCustomerSession())
            {
                return RedirectToAction("Login", "Account", new
                {
                    returnUrl = Url.Action(nameof(Index), "Wishlist")
                });
            }
            try
            {
                var response = await _apiClient.GetAsync<ApiResponse<WishlistViewModel>>("Wishlist");
                return View(response?.Data ?? new WishlistViewModel());
            }
            catch (HttpRequestException)
            {
                ViewBag.WishlistError = "Could not load your wishlist. Please try again.";
                return View(new WishlistViewModel());
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddToWishlistViewModel model)
        {
            if (!HasCustomerSession())
            {
                var returnUrl = Url.Action("Details","Product",new { id = model.ProductId });
                return RedirectToAction("Login", "Account", new { returnUrl });
            }
            if (model.ProductId <= 0)
            {
                TempData["WishlistError"] = "Invalid product.";
                return RedirectToAction("Index", "Home");
            }
            try
            {
                var response = await _apiClient.PostAsync<AddToWishlistViewModel, ApiResponse<object>>("Wishlist",model);
                if (response?.Success == true)
                {
                    TempData["WishlistSuccess"] = "Product added to your wishlist.";
                }
                else
                {
                    TempData["WishlistError"] = response?.Message ?? "Could not add this product to your wishlist.";
                }
            }
            catch (HttpRequestException)
            {
                TempData["WishlistError"] = "Could not add this product. It may be unavailable or out of stock.";
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int productId)
        {
            if (!HasCustomerSession())
            {
                return RedirectToAction("Login", "Account", new
                {
                    returnUrl = Url.Action(nameof(Index), "Wishlist")
                });
            }
            if (productId <= 0)
            {
                TempData["WishlistError"] = "Invalid product.";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                var removed = await _apiClient.DeleteAsync($"Wishlist/{productId}");
                TempData[removed ? "WishlistSuccess" : "WishlistError"] = removed
                    ? "Product removed from your wishlist."
                    : "Could not remove this product.";
            }
            catch (HttpRequestException)
            {
                TempData["WishlistError"] = "Could not remove this product. Please try again.";
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
