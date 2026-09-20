using MediatR;

namespace EventManager.Application.Common.CQRS;

public interface ICommand<out TResponse>
    : IRequest<TResponse>
{
}

public interface ICommand : IRequest {}
