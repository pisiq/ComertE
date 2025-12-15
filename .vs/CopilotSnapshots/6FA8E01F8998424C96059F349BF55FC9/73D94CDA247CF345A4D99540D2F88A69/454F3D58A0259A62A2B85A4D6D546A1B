// Services/About/AboutService.cs
using Hotel.Data;
using Hotel.Models;

namespace Hotel.Services
{
    public class AboutService : IAboutService
    {
        private readonly IAboutRepository _repository;

        public AboutService(IAboutRepository repository)
        {
            _repository = repository;
        }

        public async Task<About> GetAboutAsync()
        {
            return await _repository.GetAboutAsync();
        }

        public async Task<bool> UpdateAboutAsync(About about)
        {
            return await _repository.UpdateAboutAsync(about);
        }

        public async Task EnsureAboutExistsAsync()
        {
            var about = await _repository.GetAboutAsync();
            if (about == null)
            {
                await _repository.CreateDefaultAboutAsync();
            }
        }
    }
}
