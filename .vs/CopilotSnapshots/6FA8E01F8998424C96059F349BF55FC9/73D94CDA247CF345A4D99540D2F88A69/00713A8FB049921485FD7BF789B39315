// Controllers/AboutController.cs
using Hotel.Models;
using Hotel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AboutController : Controller
    {
        private readonly IAboutService _aboutService;

        public AboutController(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }

        public async Task<IActionResult> Edit()
        {
            var about = await _aboutService.GetAboutAsync();
            if (about == null)
            {
                await _aboutService.EnsureAboutExistsAsync();
                about = await _aboutService.GetAboutAsync();
            }

            return View(about);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(About about)
        {
            if (!ModelState.IsValid)
            {
                return View(about);
            }

            if (await _aboutService.UpdateAboutAsync(about))
            {
                TempData["SuccessMessage"] = "About page content updated successfully.";
                return RedirectToAction("Admin", "Users");
            }

            TempData["ErrorMessage"] = "Failed to update About page content.";
            return View(about);
        }
    }
}
