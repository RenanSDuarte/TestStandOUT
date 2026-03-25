using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestStandOUT.Api.Data;
using TestStandOUT.Api.Models;
using TestStandOUT.Services;

namespace TestStandOUT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeRateController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAlphaVantageService _externalApi;

        public ExchangeRateController(AppDbContext context, IAlphaVantageService externalApi)
        {
            _context = context;
            _externalApi = externalApi;
        }

        [HttpGet("{from}/{to}")]
        public async Task<IActionResult> GetRate(string from, string to)
        {
            from = from.ToUpper();
            to = to.ToUpper();

            // Try to get in the BD
            var rate = await _context.Rates
                .FirstOrDefaultAsync(r => r.BaseCurrency == from && r.QuoteCurrency == to);

            if (rate != null)
            {
                return Ok(new { Source = "Database", Data = rate });
            }

            // No find in the BD get in Alpha
            var externalData = await _externalApi.GetExchangeRateAsync(from, to);

            if (externalData == null)
                return NotFound("Tax not found.");

            // Map and save
            var newRate = new CurrencyRate
            {
                BaseCurrency = from,
                QuoteCurrency = to,
                Bid = externalData.Value.Bid,
                Ask = externalData.Value.Ask,
                LastUpdated = DateTime.UtcNow
            };

            _context.Rates.Add(newRate);
            await _context.SaveChangesAsync();

            return Ok(new { Source = "External API (AlphaVantage)", Data = newRate });
        }
    }
}
