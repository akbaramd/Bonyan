namespace Bonyan.Messaging.Abstractions;

public interface IBonMessageDispatcher
{
    Task SendAsync<TMessage>(string serviceName, TMessage message, CancellationToken cancellationToken = default)
        where TMessage : IBonMessage;

    Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : IBonMessage;
}