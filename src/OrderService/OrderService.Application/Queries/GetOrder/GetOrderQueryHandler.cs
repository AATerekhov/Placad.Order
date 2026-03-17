using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Aggregates;
using OrderService.Domain.Repositories;
using OrderService.Domain.ValueObjects;

namespace OrderService.Application.Queries.GetOrder;

internal sealed class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderDto?>
{
    private readonly IOrderRepository _repository;

    public GetOrderQueryHandler(IOrderRepository repository) => _repository = repository;

    public async Task<OrderDto?> Handle(GetOrderQuery query, CancellationToken ct)
    {
        var order = await _repository.GetByIdAsync(OrderId.From(query.OrderId), ct);
        return order is null ? null : MapToDto(order);
    }

    internal static OrderDto MapToDto(Order o) => new(
        o.Id.Value,
        o.CustomerId.Value,
        o.Status.ToString(),
        o.OrderLines.Select(l => new OrderLineDto(
            l.Id,
            l.ProductId.Value,
            l.ProductName,
            l.Quantity,
            l.Price.Amount,
            l.Price.Currency)).ToList(),
        o.TotalAmount.Amount,
        o.TotalAmount.Currency,
        o.PaymentReference?.Value,
        o.CreatedAt);
}
