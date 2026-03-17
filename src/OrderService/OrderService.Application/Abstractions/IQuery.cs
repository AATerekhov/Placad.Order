using MediatR;

namespace OrderService.Application.Abstractions;

public interface IQuery<TResult> : IRequest<TResult> { }
