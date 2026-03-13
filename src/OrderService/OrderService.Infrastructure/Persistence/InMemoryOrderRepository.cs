using System.Collections.Concurrent;
using System.Text.Json;
using OrderService.Domain.Aggregates;
using OrderService.Domain.Abstractions;
using OrderService.Domain.Repositories;
using OrderService.Domain.ValueObjects;
using OrderService.Infrastructure.Persistence.Outbox;

namespace OrderService.Infrastructure.Persistence;

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _store = new();
    private readonly InMemoryOutboxStore _outbox;

    public InMemoryOrderRepository(InMemoryOutboxStore outbox) => _outbox = outbox;

    public Task<Order?> GetByIdAsync(OrderId id, CancellationToken ct = default) =>
        Task.FromResult(_store.TryGetValue(id.Value, out var order) ? order : null);

    public Task<IEnumerable<Order>> GetByCustomerIdAsync(CustomerId customerId, CancellationToken ct = default) =>
        Task.FromResult(_store.Values.Where(o => o.CustomerId == customerId));

    public Task AddAsync(Order order, CancellationToken ct = default)
    {
        _store[order.Id.Value] = order;
        SaveOutboxMessages(order);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Order order, CancellationToken ct = default)
    {
        _store[order.Id.Value] = order;
        SaveOutboxMessages(order);
        return Task.CompletedTask;
    }

    private void SaveOutboxMessages(Order order)
    {
        foreach (var domainEvent in order.DomainEvents)
        {
            _outbox.Enqueue(new OutboxMessage
            {
                EventType = domainEvent.GetType().AssemblyQualifiedName!,
                Payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                OccurredAt = domainEvent.OccurredAt
            });
        }
        order.ClearDomainEvents();
    }
}
