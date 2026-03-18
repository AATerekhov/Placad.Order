using Microsoft.Extensions.Logging;
using Orders.Infrastructure.Persistence.Outbox.Abstractions;

namespace Orders.Infrastructure.Persistence.Outbox.Implementations;

public class LoggingIntegrationEventPublisher(ILogger<LoggingIntegrationEventPublisher> logger) : IIntegrationEventPublisher
{
    public Task PublishAsync(string eventType, string payload, CancellationToken cancellationToken)
    {
        logger.LogInformation("Published integration event. Type: {EventType}, Payload: {Payload}", eventType, payload);
        return Task.CompletedTask;
    }
}
