namespace Orders.Infrastructure.Persistence.Outbox.Abstractions;

public interface IIntegrationEventPublisher
{
    Task PublishAsync(string eventType, string payload, CancellationToken cancellationToken);
}
