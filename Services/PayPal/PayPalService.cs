using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;
using Microsoft.AspNetCore.Http;

namespace Hotel.Services
{
    public class PayPalService : IPayPalService
    {
        private readonly PayPalHttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PayPalService> _logger;

        public PayPalService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<PayPalService> logger)
        {
            var mode = configuration["PayPal:Mode"];
            var clientId = configuration["PayPal:ClientId"];
            var clientSecret = configuration["PayPal:ClientSecret"];
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;

            var environment = mode == "live"
                ? new LiveEnvironment(clientId, clientSecret)
                : (PayPalEnvironment)new SandboxEnvironment(clientId, clientSecret);

            _client = new PayPalHttpClient(environment);
        }

        public async Task<string> CreateOrderAsync(decimal amount, string currency = "USD", int bookingId = 0)
        {
            try
            {
                var request = _httpContextAccessor.HttpContext?.Request;
                var baseUrl = $"{request?.Scheme}://{request?.Host}";

                var order = new OrderRequest()
                {
                    CheckoutPaymentIntent = "CAPTURE",
                    PurchaseUnits = new List<PurchaseUnitRequest>
            {
                new PurchaseUnitRequest
                {
                    AmountWithBreakdown = new AmountWithBreakdown
                    {
                        CurrencyCode = currency,
                        Value = amount.ToString("F2")
                    }
                }
            },
                    ApplicationContext = new ApplicationContext
                    {
                        ReturnUrl = $"{baseUrl}/Booking/PaymentSuccess?bookingId={bookingId}",
                        CancelUrl = $"{baseUrl}/Booking/PaymentCancelled?bookingId={bookingId}"
                    }
                };

                var requestOrder = new OrdersCreateRequest();
                requestOrder.Prefer("return=representation");
                requestOrder.RequestBody(order);

                _logger.LogInformation("Creating PayPal order for amount: {Amount} {Currency}, Booking: {BookingId}", amount, currency, bookingId);

                var response = await _client.Execute(requestOrder).ConfigureAwait(false);
                var result = response.Result<Order>();

                _logger.LogInformation("PayPal order created successfully: {OrderId} for booking {BookingId}", result.Id, bookingId);

                return result.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create PayPal order for amount: {Amount}, Booking: {BookingId}", amount, bookingId);
                throw;
            }
        }

        public async Task<bool> CaptureOrderAsync(string orderId)
        {
            var request = new OrdersCaptureRequest(orderId);
            request.RequestBody(new OrderActionRequest());

            try
            {
                _logger.LogInformation("Capturing PayPal order: {OrderId}", orderId);

                var response = await _client.Execute(request).ConfigureAwait(false);

                _logger.LogInformation("PayPal capture response received for order: {OrderId}, StatusCode: {StatusCode}",
                    orderId, response.StatusCode);

                var result = response.Result<Order>();

                var isCompleted = result.Status == "COMPLETED";
                _logger.LogInformation("PayPal order {OrderId} capture result: {Status}, IsCompleted: {IsCompleted}",
                    orderId, result.Status, isCompleted);

                return isCompleted;
            }
            catch (HttpException httpEx)
            {
                _logger.LogError(httpEx, "HTTP error capturing PayPal order: {OrderId}, StatusCode: {StatusCode}, Message: {Message}",
                    orderId, httpEx.StatusCode, httpEx.Message);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to capture PayPal order: {OrderId}, Exception Type: {ExceptionType}",
                    orderId, ex.GetType().Name);
                return false;
            }
        }
    }
}