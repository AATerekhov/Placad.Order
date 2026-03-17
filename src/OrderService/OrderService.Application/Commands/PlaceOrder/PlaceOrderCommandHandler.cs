using MediatR;
using OrderService.Domain.Aggregates;
using OrderService.Domain.Repositories;
using OrderService.Domain.ValueObjects;

namespace OrderService.Application.Commands.PlaceOrder;

internal sealed class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, Guid>
{
    private readonly IOrderRepository _repository;

    public PlaceOrderCommandHandler(IOrderRepository repository) => _repository = repository;

    public async Task<Guid> Handle(PlaceOrderCommand cmd, CancellationToken ct)
    {
        var customerId = CustomerId.From(cmd.CustomerId);

        var lines = cmd.OrderLines.Select(l =>
            new OrderLineData(l.ProductId, l.ProductName, l.Quantity, l.Price, l.Currency));

        var order = Order.Place(customerId, lines);

        await _repository.AddAsync(order, ct);

        return order.Id.Value;
    }
}
