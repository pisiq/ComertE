// Data/About/AboutRepository.cs
using Hotel.Models;
using Hotel.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Data
{
    public class AboutRepository : IAboutRepository
    {
        private readonly HotelContext _context;

        public AboutRepository(HotelContext context)
        {
            _context = context;
        }

        public async Task<About> GetAboutAsync()
        {
            return await _context.About.FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAboutAsync(About about)
        {
            try
            {
                _context.Update(about);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CreateDefaultAboutAsync()
        {
            // Check if about content already exists
            if (await _context.About.AnyAsync())
                return true;

            // Create default about content
            var defaultAbout = new About
            {
                Description = "<h2>Welcome to Luxury Hotel</h2>"
            };

            try
            {
                _context.About.Add(defaultAbout);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
