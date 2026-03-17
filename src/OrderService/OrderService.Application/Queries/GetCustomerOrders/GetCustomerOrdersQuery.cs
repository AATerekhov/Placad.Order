using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;

namespace OrderService.Application.Queries.GetCustomerOrders;

public record GetCustomerOrdersQuery(Guid CustomerId) : IQuery<IEnumerable<OrderDto>>;
