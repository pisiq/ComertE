// ViewsModels/RestaurantViewModel.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hotel.ViewsModels
{
    public class RestaurantViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a location")]
        [Display(Name = "Location")]
        public int LocationId { get; set; }

        [Required(ErrorMessage = "Please select a dish")]
        [Display(Name = "Dish")]
        public int DishId { get; set; }

        // For dropdown lists in views
        public SelectList Locations { get; set; }
        public SelectList Dishes { get; set; }

        // For display purposes
        [Display(Name = "Location Name")]
        public string? LocationName { get; set; }

        [Display(Name = "Dish Name")]
        public string? DishName { get; set; }
    }
}
