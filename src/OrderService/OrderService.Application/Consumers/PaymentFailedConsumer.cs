using MassTransit;
using MediatR;
using OrderService.Application.Commands.FailOrder;

namespace OrderService.Application.Consumers;

public record PaymentFailed(Guid OrderId, string Reason);

public sealed class PaymentFailedConsumer : IConsumer<PaymentFailed>
{
    private readonly IMediator _mediator;

    public PaymentFailedConsumer(IMediator mediator) => _mediator = mediator;

    public Task Consume(ConsumeContext<PaymentFailed> context) =>
        _mediator.Send(
            new FailOrderCommand(context.Message.OrderId, context.Message.Reason),
            context.CancellationToken);
}
