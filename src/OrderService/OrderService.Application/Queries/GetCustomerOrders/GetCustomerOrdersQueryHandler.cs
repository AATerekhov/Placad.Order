using MediatR;
using OrderService.Application.DTOs;
using OrderService.Application.Queries.GetOrder;
using OrderService.Domain.Repositories;
using OrderService.Domain.ValueObjects;

namespace OrderService.Application.Queries.GetCustomerOrders;

internal sealed class GetCustomerOrdersQueryHandler : IRequestHandler<GetCustomerOrdersQuery, IEnumerable<OrderDto>>
{
    private readonly IOrderRepository _repository;

    public GetCustomerOrdersQueryHandler(IOrderRepository repository) => _repository = repository;

    public async Task<IEnumerable<OrderDto>> Handle(GetCustomerOrdersQuery query, CancellationToken ct)
    {
        var orders = await _repository.GetByCustomerIdAsync(CustomerId.From(query.CustomerId), ct);
        return orders.Select(GetOrderQueryHandler.MapToDto);
    }
}
