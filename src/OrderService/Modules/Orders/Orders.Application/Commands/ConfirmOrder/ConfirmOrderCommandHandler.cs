using MediatR;
using Orders.Application.Abstractions;
using Orders.Domain.Repositories;
using Orders.Domain.ValueObjects;

namespace Orders.Application.Commands.ConfirmOrder;

internal sealed class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand>
{
    private readonly IOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmOrderCommandHandler(IOrderRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ConfirmOrderCommand cmd, CancellationToken ct)
    {
        var order = await _repository.GetByIdAsync(OrderId.From(cmd.OrderId), ct)
            ?? throw new InvalidOperationException($"Order {cmd.OrderId} not found.");

        order.Confirm(PaymentReference.From(cmd.PaymentReference));

        await _repository.UpdateAsync(order, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
