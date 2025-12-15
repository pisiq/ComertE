using Microsoft.AspNetCore.Identity;
namespace Hotel.Models
{
    public class User : IdentityUser
    {      
        public string Name { get; set; }
        public int Phone { get; set; }
        public string ProfilePicturePath { get; set; }
    }
}
