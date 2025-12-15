// Controllers/DishController.cs
using Hotel.Models;
using Hotel.Services;
using Hotel.ViewsModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Hotel.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DishController : Controller
    {
        private readonly IDishService _dishService;

        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }

        // GET: Dish
        public async Task<IActionResult> Index()
        {
            var dishes = await _dishService.GetAllDishesAsync();
            return View(dishes);
        }

        // GET: Dish/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var dish = await _dishService.GetDishByIdAsync(id);
            if (dish == null)
            {
                return NotFound();
            }

            return View(dish);
        }

        // GET: Dish/Create
        public IActionResult Create()
        {
            return View(new DishViewModel());
        }

        // POST: Dish/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DishViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dish = new Dish
                    {
                        Name = viewModel.Name,
                        Price = viewModel.Price,
                        Type = viewModel.Type
                    };

                    await _dishService.AddDishAsync(dish);
                    TempData["SuccessMessage"] = "Dish created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Failed to create dish: {ex.Message}");
                }
            }

            return View(viewModel);
        }

        // GET: Dish/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var dish = await _dishService.GetDishByIdAsync(id);
            if (dish == null)
            {
                return NotFound();
            }

            var viewModel = new DishViewModel
            {
                Id = dish.Id,
                Name = dish.Name,
                Price = dish.Price,
                Type = dish.Type
            };

            return View(viewModel);
        }

        // POST: Dish/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DishViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var dish = new Dish
                    {
                        Id = viewModel.Id,
                        Name = viewModel.Name,
                        Price = viewModel.Price,
                        Type = viewModel.Type
                    };

                    var success = await _dishService.UpdateDishAsync(dish);
                    if (!success)
                    {
                        return NotFound();
                    }

                    TempData["SuccessMessage"] = "Dish updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Failed to update dish: {ex.Message}");
                }
            }

            return View(viewModel);
        }

        // GET: Dish/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var dish = await _dishService.GetDishByIdAsync(id);
            if (dish == null)
            {
                return NotFound();
            }

            return View(dish);
        }

        // POST: Dish/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _dishService.DeleteDishAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Dish deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete dish.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting dish: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
