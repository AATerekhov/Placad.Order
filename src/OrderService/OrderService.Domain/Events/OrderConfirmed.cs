using OrderService.Domain.Abstractions;
using OrderService.Domain.ValueObjects;

namespace OrderService.Domain.Events;

public sealed record OrderConfirmed(OrderId OrderId, PaymentReference PaymentReference, DateTime ConfirmedAt) : DomainEvent;
