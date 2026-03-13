using MediatR;
using OrderService.Domain.Repositories;
using OrderService.Domain.ValueObjects;

namespace OrderService.Application.Commands.FailOrder;

internal sealed class FailOrderCommandHandler : IRequestHandler<FailOrderCommand>
{
    private readonly IOrderRepository _repository;

    public FailOrderCommandHandler(IOrderRepository repository) => _repository = repository;

    public async Task Handle(FailOrderCommand cmd, CancellationToken ct)
    {
        var orderId = OrderId.From(cmd.OrderId);
        var order = await _repository.GetByIdAsync(orderId, ct)
            ?? throw new InvalidOperationException($"Order {cmd.OrderId} not found.");

        order.Fail(cmd.Reason);

        await _repository.UpdateAsync(order, ct);
    }
}
