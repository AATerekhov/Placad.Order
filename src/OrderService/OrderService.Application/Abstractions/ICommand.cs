using MediatR;

namespace OrderService.Application.Abstractions;

public interface ICommand : IRequest { }

public interface ICommand<TResult> : IRequest<TResult> { }
