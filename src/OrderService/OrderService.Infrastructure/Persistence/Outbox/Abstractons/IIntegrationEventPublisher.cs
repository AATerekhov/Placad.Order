namespace OrderService.Infrastructure.Persistence.Outbox.Abstractons
{
    public interface IIntegrationEventPublisher
    {        Task PublishAsync(string eventType, string payload, CancellationToken cancellationToken);
    }
}
