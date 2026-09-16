namespace EventManager.Application.Common.CQRS;

public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<TResponse> Handle(
        TCommand command,
        CancellationToken cancellationToken);
}
