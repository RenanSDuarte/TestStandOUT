namespace TestStandOUT.Api.Services
{
    public interface IAlphaVantageService
    {
        Task<(decimal Bid, decimal Ask)?> GetExchangeRateAsync(string fromCurrency, string toCurrency);
    }
}
