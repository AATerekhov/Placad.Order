using Microsoft.EntityFrameworkCore.Diagnostics;
using OrderService.Domain.Abstractions;
using OrderService.Domain.ValueObjects;
using OrderService.Infrastructure.Persistence.Outbox;
using OrderService.Infrastructure.Serialization;
using System.Text.Json;

namespace OrderService.Infrastructure.Persistence.Interceptors
{
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
                .ToList();//получает entity, у которых есть необработанные события.

            if(aggregates.Count == 0)
                return base.SavingChangesAsync(eventData, result,cancellationToken);

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
}
