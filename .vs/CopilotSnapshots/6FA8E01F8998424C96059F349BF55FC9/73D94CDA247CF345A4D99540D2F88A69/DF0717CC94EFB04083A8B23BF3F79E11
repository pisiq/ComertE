// Services/ContactSettings/ContactSettingsService.cs
using Hotel.Data;


namespace Hotel.Services
{
    public class ContactSettingsService : IContactSettingsService
    {
        private readonly IContactSettingsRepository _repository;

        public ContactSettingsService(IContactSettingsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Models.ContactSettings> GetContactSettingsAsync()
        {
            return await _repository.GetContactSettingsAsync();
        }

        public async Task<bool> UpdateContactSettingsAsync(Models.ContactSettings contactSettings)
        {
            return await _repository.UpdateContactSettingsAsync(contactSettings);
        }

        public async Task EnsureContactSettingsExistAsync()
        {
            var settings = await _repository.GetContactSettingsAsync();
            if (settings == null)
            {
                await _repository.CreateDefaultContactSettingsAsync();
            }
        }
    }
}
