using Bonyan.Messaging.Abstractions;

namespace Bonyan.Messaging.Abstractions;

public interface IBonMessageBus 
{
    /// <summary>
    /// Sends a message request to a specific service and waits for a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="destinationServiceName">The name of the destination service.</param>
    /// <param name="request">The request message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response from the handler.</returns>
    Task<TResponse> SendAsync<TRequest, TResponse>(
        string destinationServiceName,
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : class,IMessageRequest<TResponse>
        where TResponse : class;

    /// <summary>
    /// Sends a message request to a specific service without waiting for a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <param name="destinationServiceName">The name of the destination service.</param>
    /// <param name="request">The request message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task SendAsync<TRequest>(
        string destinationServiceName,
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : class,IMessageRequest;    

    /// <summary>
    /// Publishes a message event to multiple subscribers.
    /// </summary>
    /// <param name="messageEvent">The event message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task PublishAsync(
        IMessageEvent messageEvent,
        CancellationToken cancellationToken = default);
}