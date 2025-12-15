// ViewsModels/DishViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace Hotel.ViewsModels
{
    public class DishViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Dish Name")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }


        [Required(ErrorMessage = "Type is required")]
        [Display(Name = "Dish Type")]
        [StringLength(50, ErrorMessage = "Type cannot be longer than 50 characters")]
        public string Type { get; set; }
    }
}