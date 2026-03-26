namespace TestStandOUT.Api.Events;

public record CurrencyRateCreated(int Id, string Pair, decimal Bid, DateTime CreatedAt);