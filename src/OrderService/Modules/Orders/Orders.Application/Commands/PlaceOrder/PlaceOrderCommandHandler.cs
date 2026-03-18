using MediatR;
using Orders.Application.Abstractions;
using Orders.Domain.Aggregates;
using Orders.Domain.Repositories;
using Orders.Domain.ValueObjects;

namespace Orders.Application.Commands.PlaceOrder;

internal sealed class PlaceOrderCommandHandler(IOrderRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<PlaceOrderCommand, Guid>
{
    private readonly IOrderRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Guid> Handle(PlaceOrderCommand cmd, CancellationToken ct)
    {
        var customerId = CustomerId.From(cmd.CustomerId);

        var lines = cmd.OrderLines.Select(l =>
            new OrderLineData(l.ProductId, l.ProductName, l.Quantity, l.Price, l.Currency));

        var order = Order.Place(customerId, lines);

        await _repository.AddAsync(order, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return order.Id.Value;
    }
}
