using System.Globalization;
using System.Text.Json;

namespace TestStandOUT.Services
{
    public class AlphaVantageService : IAlphaVantageService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "API_KEY";

        public AlphaVantageService(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<(decimal Bid, decimal Ask)?> GetExchangeRateAsync(string from, string to)
        {
            var url = $"query?function=CURRENCY_EXCHANGE_RATE&from_currency={from}&to_currency={to}&apikey={_apiKey}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);

            // Return Json
            // Search 
            if (doc.RootElement.TryGetProperty("Realtime Currency Exchange Rate", out var data))
            {
                var rate = decimal.Parse(data.GetProperty("5. Exchange Rate").GetString(),CultureInfo.InvariantCulture);
                // Simulation
                return (rate * 0.9995m, rate * 1.0005m);
            }

            return null;
        }
    }
}
