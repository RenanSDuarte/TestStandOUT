namespace TestStandOUT.Api.Models;

public class CurrencyRate
{
    public int Id { get; set; }
    public string BaseCurrency { get; set; } = string.Empty;
    public string QuoteCurrency { get; set; } = string.Empty;
    public decimal Bid { get; set; }
    public decimal Ask { get; set; }
    public DateTime LastUpdated { get; set; }
}