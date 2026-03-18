using MediatR;
using Orders.Application.Abstractions;
using Orders.Domain.Repositories;
using Orders.Domain.ValueObjects;

namespace Orders.Application.Commands.FailOrder;

internal sealed class FailOrderCommandHandler : IRequestHandler<FailOrderCommand>
{
    private readonly IOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public FailOrderCommandHandler(IOrderRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(FailOrderCommand cmd, CancellationToken ct)
    {
        var order = await _repository.GetByIdAsync(OrderId.From(cmd.OrderId), ct)
            ?? throw new InvalidOperationException($"Order {cmd.OrderId} not found.");

        order.Fail(cmd.Reason);

        await _repository.UpdateAsync(order, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
