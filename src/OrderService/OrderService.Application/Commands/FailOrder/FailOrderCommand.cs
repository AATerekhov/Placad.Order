using OrderService.Application.Abstractions;

namespace OrderService.Application.Commands.FailOrder;

public record FailOrderCommand(Guid OrderId, string Reason) : ICommand;
