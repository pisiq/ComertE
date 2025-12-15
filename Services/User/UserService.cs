using Hotel.Models;
using Hotel.ViewsModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Hotel.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<bool> RegisterUserAsync(SignModel model)
        {
            // Check if email exists
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return false; // Email already in use
            }

            var user = new User
            {
                UserName = model.Email, // Required by Identity
                Email = model.Email,
                Name = model.Name,
                Phone = model.Phone,
                ProfilePicturePath = "/images/profiles/default-avatar.png"
            };

            // Create user with Identity (handles password hashing)
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Ensure role exists
                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole("User"));
                    if (!roleResult.Succeeded)
                    {
                        // Handle role creation error
                        return true; // Still return true since user was created
                    }
                }

                // Add user to role
                var addToRoleResult = await _userManager.AddToRoleAsync(user, "User");

                // We'll still return true even if role assignment fails
                // because the user account was successfully created
                return true;
            }

            return false;
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<bool> AuthenticateUserAsync(LoginModel model)
        {
            // First find the user by email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return false;
            }

            // Use SignInManager to sign in the user
            var result = await _signInManager.PasswordSignInAsync(
                model.Email,  // username (same as email in this case)
                model.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            return result.Succeeded;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
    }
}
