using Microsoft.EntityFrameworkCore.Diagnostics;
using Orders.Domain.ValueObjects;
using Orders.Infrastructure.Persistence.Outbox;
using Orders.Infrastructure.Serialization;
using SharedKernel;
using System.Text.Json;

namespace Orders.Infrastructure.Persistence.Interceptors;

public sealed class ConvertDomainEventsToOutboxMessagesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;
        if (dbContext is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var aggregates = dbContext.ChangeTracker
            .Entries<AggregateRoot<OrderId>>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        if (aggregates.Count == 0)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var outboxMessages = new List<OutboxMessage>();

        foreach (var aggregate in aggregates)
        {
            foreach (var domainEvent in aggregate.DomainEvents)
            {
                var outboxMessage = OutboxMessage.Create(
                    occurredAt: domainEvent.OccurredAt,
                    type: domainEvent.GetType().AssemblyQualifiedName!,
                    payload: JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), JsonOptionsProvider.Default));

                outboxMessages.Add(outboxMessage);
            }

            aggregate.ClearDomainEvents();
        }

        dbContext.Set<OutboxMessage>().AddRange(outboxMessages);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
