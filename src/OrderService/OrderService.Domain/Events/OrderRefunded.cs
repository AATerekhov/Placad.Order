using OrderService.Domain.Abstractions;
using OrderService.Domain.ValueObjects;

namespace OrderService.Domain.Events;

public sealed record OrderRefunded(OrderId OrderId, Money RefundedAmount, DateTime RefundedAt) : DomainEvent;
