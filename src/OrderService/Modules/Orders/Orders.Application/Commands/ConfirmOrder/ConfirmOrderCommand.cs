using Orders.Application.Abstractions;

namespace Orders.Application.Commands.ConfirmOrder;

public record ConfirmOrderCommand(Guid OrderId, string PaymentReference) : ICommand;
