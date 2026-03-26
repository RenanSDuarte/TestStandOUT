using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestStandOUT.Api.Data;
using TestStandOUT.Api.Events;
using TestStandOUT.Api.Models;
using TestStandOUT.Api.Services;

namespace TestStandOUT.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeRateController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAlphaVantageService _externalApi;
        private readonly IPublishEndpoint _publishEndpoint;

        public ExchangeRateController(AppDbContext context, IAlphaVantageService externalApi, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _externalApi = externalApi;
            _publishEndpoint = publishEndpoint;
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

            await _publishEndpoint.Publish(new CurrencyRateCreated(
                newRate.Id,
                $"{newRate.BaseCurrency}/{newRate.QuoteCurrency}",
                newRate.Bid,
                DateTime.UtcNow
            ));

            return Ok(new { Source = "External API", Data = newRate });
        }

        [HttpPost]
        public async Task<IActionResult> AddRate([FromBody] CurrencyRate newRate)
        {
            if (newRate == null) return BadRequest();

            // Verifica se já existe para evitar erro de duplicidade
            var exists = await _context.Rates.AnyAsync(r =>
                r.BaseCurrency == newRate.BaseCurrency.ToUpper() &&
                r.QuoteCurrency == newRate.QuoteCurrency.ToUpper());

            if (exists) return Conflict("Already Exist");

            newRate.BaseCurrency = newRate.BaseCurrency.ToUpper();
            newRate.QuoteCurrency = newRate.QuoteCurrency.ToUpper();
            newRate.LastUpdated = DateTime.UtcNow;

            _context.Rates.Add(newRate);
            await _context.SaveChangesAsync();

            await _publishEndpoint.Publish(new CurrencyRateCreated(
                newRate.Id,
                $"{newRate.BaseCurrency}/{newRate.QuoteCurrency}",
                newRate.Bid,
                DateTime.UtcNow
            ));

            return Ok(new { Source = "Add directly in the BD", Data = newRate });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRate(int id, [FromBody] CurrencyRate updatedRate)
        {
            var rate = await _context.Rates.FindAsync(id);
            if (rate == null) return NotFound();

            // Atualiza apenas os valores permitidos
            rate.Bid = updatedRate.Bid;
            rate.Ask = updatedRate.Ask;
            rate.LastUpdated = DateTime.UtcNow;

            _context.Entry(rate).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(rate);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRate(int id)
        {
            var rate = await _context.Rates.FindAsync(id);
            if (rate == null) return NotFound();

            _context.Rates.Remove(rate);
            await _context.SaveChangesAsync();

            return NoContent(); 
        }
    }
}
