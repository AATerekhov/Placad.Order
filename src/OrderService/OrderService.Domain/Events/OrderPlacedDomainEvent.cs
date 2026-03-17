using OrderService.Domain.Abstractions;
using OrderService.Domain.ValueObjects;

namespace OrderService.Domain.Events;

//Событие размещение заказа.
public sealed record OrderPlacedDomainEvent(OrderId OrderId, CustomerId CustomerId, DateTime PlacedAt) : DomainEvent;
