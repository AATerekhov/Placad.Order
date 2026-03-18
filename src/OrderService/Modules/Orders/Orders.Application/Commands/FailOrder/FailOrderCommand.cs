using Orders.Application.Abstractions;

namespace Orders.Application.Commands.FailOrder;

public record FailOrderCommand(Guid OrderId, string Reason) : ICommand;
