using OrderService.Domain.Abstractions;
using OrderService.Domain.ValueObjects;

namespace OrderService.Domain.Events;

//Событие возврата заказа.
public sealed record OrderRefunded(OrderId OrderId, Money RefundedAmount, DateTime RefundedAt) : DomainEvent;
