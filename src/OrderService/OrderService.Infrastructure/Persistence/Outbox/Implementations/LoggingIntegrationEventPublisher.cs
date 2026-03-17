
using Microsoft.Extensions.Logging;
using OrderService.Infrastructure.Persistence.Outbox.Abstractons;

namespace OrderService.Infrastructure.Persistence.Outbox.Implementations
{
    public class LoggingIntegrationEventPublisher(ILogger<LoggingIntegrationEventPublisher> logger) : IIntegrationEventPublisher
    {
        public Task PublishAsync(string eventType, string payload, CancellationToken cancellationToken)
        {
            logger.LogInformation("Published integration event. Type: {EventType}, Payload: {Payload}", eventType, payload);
            return Task.CompletedTask;
        }
    }
}
