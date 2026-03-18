using MediatR;

namespace Orders.Application.Abstractions;

public interface IQuery<TResult> : IRequest<TResult> { }
