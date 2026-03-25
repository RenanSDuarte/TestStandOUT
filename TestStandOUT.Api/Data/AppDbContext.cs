using Microsoft.EntityFrameworkCore;
using TestStandOUT.Api.Models;

namespace TestStandOUT.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<CurrencyRate> Rates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Dont duplicate pairs
            modelBuilder.Entity<CurrencyRate>()
                .HasIndex(r => new { r.BaseCurrency, r.QuoteCurrency })
                .IsUnique();
        }
    }
}