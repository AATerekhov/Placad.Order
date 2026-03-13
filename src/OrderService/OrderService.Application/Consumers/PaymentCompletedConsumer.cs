using MassTransit;
using MediatR;
using OrderService.Application.Commands.ConfirmOrder;

namespace OrderService.Application.Consumers;

public record PaymentCompleted(Guid OrderId, string PaymentReference);

public sealed class PaymentCompletedConsumer : IConsumer<PaymentCompleted>
{
    private readonly IMediator _mediator;

    public PaymentCompletedConsumer(IMediator mediator) => _mediator = mediator;

    public Task Consume(ConsumeContext<PaymentCompleted> context) =>
        _mediator.Send(
            new ConfirmOrderCommand(context.Message.OrderId, context.Message.PaymentReference),
            context.CancellationToken);
}
