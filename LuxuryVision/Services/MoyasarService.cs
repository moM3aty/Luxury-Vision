using LuxuryVision.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LuxuryVision.Services
{
    public class MoyasarService : IMoyasarService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _secretKey;

        public MoyasarService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _secretKey = _configuration["Moyasar:SecretKey"];
        }

        public async Task<string> CreatePaymentAsync(Order order)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var callbackUrl = $"{request.Scheme}://{request.Host}/Payment/PaymentCallback";

            var paymentData = new
            {
                amount = (int)(order.TotalAmount * 100), // Amount in halalas
                currency = "SAR",
                description = $"Order #{order.Id} for Luxury Vision",
                callback_url = callbackUrl,
                metadata = new { order_id = order.Id }
            };

            var client = _httpClientFactory.CreateClient();
            var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_secretKey}:"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

            var content = new StringContent(JsonSerializer.Serialize(paymentData), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.moyasar.com/v1/payments", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                using (var jsonDoc = JsonDocument.Parse(responseBody))
                {
                    // The transaction_url is inside the 'source' object for credit card payments
                    var source = jsonDoc.RootElement.GetProperty("source");
                    var transactionUrl = source.GetProperty("transaction_url").GetString();
                    return transactionUrl;
                }
            }
            else
            {
                // Handle API error
                var errorBody = await response.Content.ReadAsStringAsync();
                // You might want to log this errorBody for debugging
                return null;
            }
        }
    }
}