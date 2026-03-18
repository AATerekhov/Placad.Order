using SharedKernel;
using Orders.Domain.ValueObjects;

namespace Orders.Domain.Events;

public sealed record OrderConfirmedDomainEvent(OrderId OrderId, PaymentReference PaymentReference, DateTime ConfirmedAt) : DomainEvent;
