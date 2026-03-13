using System.Collections.Concurrent;

namespace OrderService.Infrastructure.Persistence.Outbox;

public sealed class InMemoryOutboxStore
{
    private readonly ConcurrentQueue<OutboxMessage> _queue = new();

    public void Enqueue(OutboxMessage message) => _queue.Enqueue(message);

    public IReadOnlyList<OutboxMessage> GetUnprocessed()
    {
        return _queue.Where(m => !m.IsProcessed).ToList();
    }

    public void MarkProcessed(Guid messageId)
    {
        var message = _queue.FirstOrDefault(m => m.Id == messageId);
        if (message is not null)
            message.ProcessedAt = DateTime.UtcNow;
    }
}
