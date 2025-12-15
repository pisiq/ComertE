
using Hotel.Models;
using Hotel.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Data
{
    public class ContactSettingsRepository : IContactSettingsRepository
    {
        private readonly HotelContext _context;

        public ContactSettingsRepository(HotelContext context)
        {
            _context = context;
        }

        public async Task<Models.ContactSettings> GetContactSettingsAsync()
        {
            // Typically we'd just have one contact settings entry
            return await _context.ContactSettings.FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateContactSettingsAsync(Models.ContactSettings contactSettings)
        {
            try
            {
                _context.Update(contactSettings);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CreateDefaultContactSettingsAsync()
        {
            // Check if contact settings already exist
            if (await _context.ContactSettings.AnyAsync())
                return true;

            // Create default contact settings
            var defaultSettings = new Models.ContactSettings
            {
                Email = "contact@luxuryhotel.com",
                PhoneNumber = "+40 702 222 222",
                Address = "123 Luxury Avenue, City Center, 10001",
                SupportHours = "Monday - Sunday: 8:00 AM - 10:00 PM"
            };

            try
            {
                _context.ContactSettings.Add(defaultSettings);
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
