using SharedKernel;
using Orders.Domain.ValueObjects;

namespace Orders.Domain.Events;

public sealed record OrderFailedDomainEvent(OrderId OrderId, string Reason, DateTime FailedAt) : DomainEvent;
