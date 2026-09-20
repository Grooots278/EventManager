using MediatR;

namespace EventManager.Application.Common.CQRS;

public interface IQuery<out TResponse>
    : IRequest<TResponse>
{
    
}