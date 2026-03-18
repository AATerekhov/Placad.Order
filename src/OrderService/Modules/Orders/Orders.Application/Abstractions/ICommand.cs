using MediatR;

namespace Orders.Application.Abstractions;

public interface ICommand : IRequest { }

public interface ICommand<TResult> : IRequest<TResult> { }
