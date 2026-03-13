using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;

namespace OrderService.Application.Queries.GetOrder;

public record GetOrderQuery(Guid OrderId) : IQuery<OrderDto?>;
