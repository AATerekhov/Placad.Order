using OrderService.Domain.Abstractions;
using OrderService.Domain.ValueObjects;

namespace OrderService.Domain.Events;

public sealed record OrderFailed(OrderId OrderId, string Reason, DateTime FailedAt) : DomainEvent;
