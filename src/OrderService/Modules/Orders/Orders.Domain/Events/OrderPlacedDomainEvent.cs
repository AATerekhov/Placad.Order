using SharedKernel;
using Orders.Domain.ValueObjects;

namespace Orders.Domain.Events;

public sealed record OrderPlacedDomainEvent(OrderId OrderId, CustomerId CustomerId, DateTime PlacedAt) : DomainEvent;
