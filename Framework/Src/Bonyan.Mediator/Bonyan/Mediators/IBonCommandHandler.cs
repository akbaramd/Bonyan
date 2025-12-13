namespace Bonyan.Mediators;

/// <summary>
/// Handler for commands.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IBonCommandHandler<TCommand, TResponse> where TCommand : IBonCommand<TResponse>
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Handler for commands without a response.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
public interface IBonCommandHandler<TCommand> where TCommand : IBonCommand
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Handler for events.
/// </summary>
/// <typeparam name="TEvent">The type of the event.</typeparam>
public interface IBonEventHandler<TEvent> where TEvent : IBonEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}