using Orders.Application.Abstractions;
using Orders.Application.DTOs;

namespace Orders.Application.Queries.GetCustomerOrders;

public record GetCustomerOrdersQuery(Guid CustomerId) : IQuery<IEnumerable<OrderDto>>;
