using OrderService.Domain.Abstractions;
using OrderService.Domain.ValueObjects;

namespace OrderService.Domain.Events;

//Событие неудачного/ошибки заказа.
public sealed record OrderFailed(OrderId OrderId, string Reason, DateTime FailedAt) : DomainEvent;
