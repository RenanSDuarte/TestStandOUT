using MassTransit;
using TestStandOUT.Api.Events;

namespace TestStandOUT.Api.Consumers;

public class RateCreatedConsumer : IConsumer<CurrencyRateCreated>
{
    private readonly ILogger<RateCreatedConsumer> _logger;

    public RateCreatedConsumer(ILogger<RateCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<CurrencyRateCreated> context)
    {
        // Here you can simulate what you do with the msg.
        _logger.LogInformation($"[QUEUE] New rate: {context.Message.Pair} - Value: {context.Message.Bid}");
        return Task.CompletedTask;
    }
}