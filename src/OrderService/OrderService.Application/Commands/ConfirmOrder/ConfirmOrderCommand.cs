using OrderService.Application.Abstractions;

namespace OrderService.Application.Commands.ConfirmOrder;

public record ConfirmOrderCommand(Guid OrderId, string PaymentReference) : ICommand;
