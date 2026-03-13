using MediatR;
using OrderService.Domain.Repositories;
using OrderService.Domain.ValueObjects;

namespace OrderService.Application.Commands.ConfirmOrder;

internal sealed class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand>
{
    private readonly IOrderRepository _repository;

    public ConfirmOrderCommandHandler(IOrderRepository repository) => _repository = repository;

    public async Task Handle(ConfirmOrderCommand cmd, CancellationToken ct)
    {
        var orderId = OrderId.From(cmd.OrderId);
        var order = await _repository.GetByIdAsync(orderId, ct)
            ?? throw new InvalidOperationException($"Order {cmd.OrderId} not found.");

        order.Confirm(PaymentReference.From(cmd.PaymentReference));

        await _repository.UpdateAsync(order, ct);
    }
}
