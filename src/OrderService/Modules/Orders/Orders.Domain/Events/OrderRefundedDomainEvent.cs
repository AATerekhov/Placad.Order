using SharedKernel;
using Orders.Domain.ValueObjects;

namespace Orders.Domain.Events;

public sealed record OrderRefundedDomainEvent(OrderId OrderId, Money RefundedAmount, DateTime RefundedAt) : DomainEvent;
