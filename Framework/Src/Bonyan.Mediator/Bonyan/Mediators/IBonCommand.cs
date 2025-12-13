namespace Bonyan.Mediators;

/// <summary>
/// Represents a command that can be sent through the mediator.
/// Commands are used for operations that modify state or perform actions.
/// </summary>
public interface IBonCommand 
{
    
}

/// <summary>
/// Represents a command that expects a response.
/// This interface replaces both IBonCommand and IBonQuery functionality.
/// </summary>
/// <typeparam name="TResponse">The type of the expected response.</typeparam>
public interface IBonCommand<TResponse> : IBonCommand
{
    
}