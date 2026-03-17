using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Aggregates;
using OrderService.Domain.Repositories;
using OrderService.Domain.ValueObjects;
using OrderService.Infrastructure.Persistence.Outbox;

namespace OrderService.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context) => _context = context;

    public Task<Order?> GetByIdAsync(OrderId id, CancellationToken ct = default) =>
        _context.Orders
            .Include(o => o.OrderLines)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(CustomerId customerId, CancellationToken ct = default) =>
        await _context.Orders
            .Include(o => o.OrderLines)
            .Where(o => o.CustomerId == customerId)
            .ToListAsync(ct);

    public async Task AddAsync(Order order, CancellationToken ct = default)
    {
        await _context.Orders.AddAsync(order, ct);
        await SaveChangesWithOutboxAsync(order, ct);
    }

    public async Task UpdateAsync(Order order, CancellationToken ct = default)
    {
        _context.Orders.Update(order);
        await SaveChangesWithOutboxAsync(order, ct);
    }

    private async Task SaveChangesWithOutboxAsync(Order order, CancellationToken ct)
    {
        foreach (var domainEvent in order.DomainEvents)
        {
            _context.OutboxMessages.Add(OutboxMessage.Create(
                domainEvent.OccurredAt,
                domainEvent.GetType().AssemblyQualifiedName!,
                JsonSerializer.Serialize(domainEvent, domainEvent.GetType())));
        }
        order.ClearDomainEvents();

        // Order rows + outbox rows written in one transaction
        await _context.SaveChangesAsync(ct);
    }
}
