using Microsoft.EntityFrameworkCore;
using TestStandOUT.Api.Data;
using TestStandOUT.Api.Models;
using Xunit;

namespace TestStandOUT.Tests
{
    public class CurrencyTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Nome aleatório para não misturar testes
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public void CurrencyRate_Should_Calculate_BidAsk()
        {
            var rate = new CurrencyRate { Bid = 5.10m, Ask = 5.20m };

            Assert.True(rate.Ask > rate.Bid);
            Assert.Equal(5.10m, rate.Bid);
        }

        [Fact]
        public async Task AddRate_Should_SaveToDatabase()
        {
            // Arrange
            var context = GetDbContext();
            var newRate = new CurrencyRate { BaseCurrency = "USD", QuoteCurrency = "JPY", Bid = 150.0m, Ask = 151.0m };

            // Act
            context.Rates.Add(newRate);
            await context.SaveChangesAsync();

            // Assert
            var savedRate = await context.Rates.FirstOrDefaultAsync(r => r.BaseCurrency == "USD");
            Assert.NotNull(savedRate);
            Assert.Equal(150.0m, savedRate.Bid);
        }

        [Fact]
        public async Task UpdateRate_Should_ModifyExistingValue()
        {
            // Arrange
            var context = GetDbContext();
            var rate = new CurrencyRate { Id = 1, BaseCurrency = "EUR", QuoteCurrency = "BRL", Bid = 5.40m, Ask = 5.50m };
            context.Rates.Add(rate);
            await context.SaveChangesAsync();

            // Act
            var existingRate = await context.Rates.FindAsync(1);
            existingRate.Bid = 6.00m;
            context.Rates.Update(existingRate);
            await context.SaveChangesAsync();

            // Assert
            var updated = await context.Rates.FindAsync(1);
            Assert.Equal(6.00m, updated.Bid);
        }

        [Fact]
        public async Task DeleteRate_Should_RemoveFromDatabase()
        {
            // Arrange
            var context = GetDbContext();
            var rate = new CurrencyRate { Id = 10, BaseCurrency = "GBP", QuoteCurrency = "USD", Bid = 1.25m, Ask = 1.26m };
            context.Rates.Add(rate);
            await context.SaveChangesAsync();

            // Act
            var toDelete = await context.Rates.FindAsync(10);
            context.Rates.Remove(toDelete);
            await context.SaveChangesAsync();

            // Assert
            var exists = await context.Rates.AnyAsync(r => r.Id == 10);
            Assert.False(exists);
        }
    }
}