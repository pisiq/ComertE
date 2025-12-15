using System.ComponentModel.DataAnnotations;

namespace Hotel.Models
{
    public class ContactSettings
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        public string Address { get; set; }

        [Display(Name = "Support Hours")]
        public string SupportHours { get; set; }
    }
}
