using System.Diagnostics.CodeAnalysis;
using Bonyan.Messaging.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Novin.AspNetCore.Novin.AspNetCore.Messaging
{
    public class NovinMessagingEndpointBuilder
    {
        private readonly WebApplication _app;

        public NovinMessagingEndpointBuilder(WebApplication app)
        {
            _app = app ?? throw new ArgumentNullException(nameof(app));
        }

        #region Default Method (Post - Send with/without Response)

        /// <summary>
        /// Maps a POST request for sending a message. If a response type is provided, it will return a response.
        /// If no response type is provided, it will simply send the message without a response.
        /// </summary>
        public void MapSend<TMessage, TResponse>([StringSyntax("Route")] string pattern)
            where TMessage : class
            where TResponse : class
        {
            _app.MapPost(pattern, async (HttpContext context) =>
            {
                var message = await context.Request.ReadFromJsonAsync<TMessage>();
                var messageBus = context.RequestServices.GetRequiredService<IBonMessageBus>();

                // Check if the response type is specified and return accordingly
                var response = await messageBus.SendAsync<TMessage, TResponse>("serviceName", message, cancellationToken: context.RequestAborted);
                return Results.Ok(response); // Return the response if available
            })
            .Accepts<TMessage>("application/json")
            .Produces<TResponse>(); // If TResponse is provided, this ensures the response is expected
        }

        /// <summary>
        /// Maps a POST request for sending a message without expecting a response.
        /// This method serves as the default behavior when no response is expected.
        /// </summary>
        public void MapSend<TMessage>([StringSyntax("Route")] string pattern)
            where TMessage : class
        {
            _app.MapPost(pattern, async (HttpContext context) =>
            {
                var message = await context.Request.ReadFromJsonAsync<TMessage>();
                var messageBus = context.RequestServices.GetRequiredService<IBonMessageBus>();
                await messageBus.SendAsync("serviceName", message, cancellationToken: context.RequestAborted);
                return Results.Ok(); // No response is returned, just an acknowledgment
            })
            .Accepts<TMessage>("application/json");
        }

        #endregion

        #region Publish Method

        /// <summary>
        /// Maps a POST request for publishing a message without expecting a response.
        /// This is for cases where the message is being published to subscribers, not expecting a reply.
        /// </summary>
        public void MapPublish<TMessage>([StringSyntax("Route")] string pattern)
            where TMessage : class
        {
            _app.MapPost(pattern, async (HttpContext context) =>
            {
                var message = await context.Request.ReadFromJsonAsync<TMessage>();
                var messageBus = context.RequestServices.GetRequiredService<IBonMessageBus>();
                await messageBus.PublishAsync(message, cancellationToken: context.RequestAborted);
                return Results.Ok(); // Return OK status since it's a publish operation
            })
            .Accepts<TMessage>("application/json");
        }

        #endregion
    }
}
