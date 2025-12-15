namespace Hotel.Services
{
    public interface IPayPalService
    {
        Task<string> CreateOrderAsync(decimal amount, string currency = "USD", int bookingId = 0);
        Task<bool> CaptureOrderAsync(string orderId);
    }
}