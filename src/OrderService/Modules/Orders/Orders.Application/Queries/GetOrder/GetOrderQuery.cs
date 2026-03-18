using Orders.Application.Abstractions;
using Orders.Application.DTOs;

namespace Orders.Application.Queries.GetOrder;

public record GetOrderQuery(Guid OrderId) : IQuery<OrderDto?>;
