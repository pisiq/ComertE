// Controllers/ContactSettingsController.cs
using Hotel.Models;
using Hotel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ContactSettingsController : Controller
    {
        private readonly IContactSettingsService _contactSettingsService;

        public ContactSettingsController(IContactSettingsService contactSettingsService)
        {
            _contactSettingsService = contactSettingsService;
        }

        public async Task<IActionResult> Edit()
        {
            var contactSettings = await _contactSettingsService.GetContactSettingsAsync();
            if (contactSettings == null)
            {
                await _contactSettingsService.EnsureContactSettingsExistAsync();
                contactSettings = await _contactSettingsService.GetContactSettingsAsync();
            }

            return View(contactSettings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ContactSettings contactSettings)
        {
            if (!ModelState.IsValid)
            {
                return View(contactSettings);
            }

            if (await _contactSettingsService.UpdateContactSettingsAsync(contactSettings))
            {
                TempData["SuccessMessage"] = "Contact settings updated successfully.";
                return RedirectToAction("Admin", "Users");
            }

            TempData["ErrorMessage"] = "Failed to update contact settings.";
            return View(contactSettings);
        }
    }
}
