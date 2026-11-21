using AmazonClone.Shared.Responses;
using AmzonClone.Web.Models.Address;
using AmzonClone.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AmzonClone.Web.Controllers
{
    public class AddressController : Controller
    {
        private readonly IApiClient _apiClient;
        public AddressController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!HasCustomerSession())
                return RedirectToLogin();

            try
            {
                var response =
                    await _apiClient.GetAsync<ApiResponse<List<AddressViewModel>>>(
                        "Address");

                return View(response?.Data ?? new List<AddressViewModel>());
            }
            catch (HttpRequestException)
            {
                ViewBag.AddressError =
                    "Could not load your saved addresses. Please try again.";

                return View(new List<AddressViewModel>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!HasCustomerSession())
                return RedirectToLogin();

            return View("Form", new AddressViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddressViewModel model)
        {
            if (!HasCustomerSession())
                return RedirectToLogin();

            if (!ModelState.IsValid)
                return View("Form", model);

            try
            {
                var response = await _apiClient.PostAsync<
                    AddressRequest,
                    ApiResponse<AddressViewModel>>(
                    "Address",
                    ToAddressRequest(model));

                if (response?.Success == true)
                {
                    TempData["AddressSuccess"] = "Address added successfully.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.FormError =
                    response?.Message ?? "Could not save the address.";
            }
            catch (HttpRequestException)
            {
                ViewBag.FormError =
                    "Could not save the address. Please try again.";
            }

            return View("Form", model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!HasCustomerSession())
                return RedirectToLogin();

            if (id <= 0)
                return BadRequest();

            try
            {
                var response =
                    await _apiClient.GetAsync<ApiResponse<List<AddressViewModel>>>(
                        "Address");

                var address = response?.Data?.FirstOrDefault(a => a.Id == id);

                if (address is null)
                    return NotFound();

                return View("Form", address);
            }
            catch (HttpRequestException)
            {
                TempData["AddressError"] =
                    "Could not load the address. Please try again.";

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AddressViewModel model)
        {
            if (!HasCustomerSession())
                return RedirectToLogin();

            if (model.Id <= 0)
                return BadRequest();

            if (!ModelState.IsValid)
                return View("Form", model);

            try
            {
                var request = new UpdateAddressRequest
                {
                    Id = model.Id,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    AddressLine1 = model.AddressLine1,
                    AddressLine2 = model.AddressLine2,
                    City = model.City,
                    State = model.State,
                    PostalCode = model.PostalCode,
                    Country = model.Country,
                    IsDefault = model.IsDefault
                };

                var response = await _apiClient.PutAsync<
                    UpdateAddressRequest,
                    ApiResponse<AddressViewModel>>(
                    "Address",
                    request);

                if (response?.Success == true)
                {
                    TempData["AddressSuccess"] = "Address updated successfully.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.FormError =
                    response?.Message ?? "Could not update the address.";
            }
            catch (HttpRequestException)
            {
                ViewBag.FormError =
                    "Could not update the address. Please try again.";
            }

            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefault(int id)
        {
            if (!HasCustomerSession())
                return RedirectToLogin();

            if (id <= 0)
            {
                TempData["AddressError"] = "Invalid address.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var response = await _apiClient.PutAsync<
                    Dictionary<string, string>,
                    ApiResponse<object>>(
                    $"Address/{id}/set-default",
                    new Dictionary<string, string>());

                TempData[response?.Success == true
                    ? "AddressSuccess"
                    : "AddressError"] =
                    response?.Message ?? "Could not update the default address.";
            }
            catch (HttpRequestException)
            {
                TempData["AddressError"] =
                    "Could not update the default address. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!HasCustomerSession())
                return RedirectToLogin();

            if (id <= 0)
            {
                TempData["AddressError"] = "Invalid address.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var deleted = await _apiClient.DeleteAsync($"Address/{id}");

                TempData[deleted ? "AddressSuccess" : "AddressError"] = deleted
                    ? "Address deleted successfully."
                    : "Could not delete the address.";
            }
            catch (HttpRequestException)
            {
                TempData["AddressError"] =
                    "Could not delete the address. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool HasCustomerSession()
        {
            return !string.IsNullOrWhiteSpace(
                HttpContext.Session.GetString("CustomerAccessToken"));
        }

        private IActionResult RedirectToLogin()
        {
            return RedirectToAction(
                "Login",
                "Account",
                new { returnUrl = Url.Action(nameof(Index), "Address") });
        }

        private static AddressRequest ToAddressRequest(AddressViewModel model)
        {
            return new AddressRequest
            {
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                City = model.City,
                State = model.State,
                PostalCode = model.PostalCode,
                Country = model.Country,
                IsDefault = model.IsDefault
            };
        }
    }
}
