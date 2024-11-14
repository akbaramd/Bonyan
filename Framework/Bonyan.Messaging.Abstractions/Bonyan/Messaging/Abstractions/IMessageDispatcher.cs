using System;
using System.Threading;
using System.Threading.Tasks;
using Bonyan.Messaging.Abstractions;

namespace Bonyan.Messaging
{
    public interface IMessageDispatcher
    {
        /// <summary>
        /// Sends a message to a specific service or consumer.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="message">The message to be sent.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) 
            where TMessage : IBonMessage;

        /// <summary>
        /// Publishes a message to all consumers of that message type.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="message">The message to be published.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) 
            where TMessage : IBonMessage;
    }
}