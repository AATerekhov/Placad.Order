namespace Orders.Infrastructure.Persistence.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string EventType { get; init; } = string.Empty;
    public string Payload { get; init; } = string.Empty;
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    public string? Error { get; set; }

    public bool IsProcessed => ProcessedAt.HasValue;

    public static OutboxMessage Create(DateTime occurredAt, string type, string payload)
    {
        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            OccurredAt = occurredAt,
            EventType = type,
            Payload = payload
        };
    }
}
