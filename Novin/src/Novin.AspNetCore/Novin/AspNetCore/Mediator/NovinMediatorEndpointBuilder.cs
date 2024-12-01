using System.Diagnostics.CodeAnalysis;
using Bonyan.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Novin.AspNetCore.Novin.AspNetCore.Mediator
{
  

    public class NovinMediatorEndpointBuilder
    {
        private readonly WebApplication _nonEndpointBuilder;

        public NovinMediatorEndpointBuilder(WebApplication nonEndpointBuilder)
        {
            _nonEndpointBuilder = nonEndpointBuilder ?? throw new ArgumentNullException(nameof(nonEndpointBuilder));
        }

        #region Command Methods

        /// <summary>
        /// Maps a POST command with response.
        /// </summary>
        public void MapPostCommand<TCommand, TResponse>([StringSyntax("Route")] string pattern)
            where TCommand : IBonCommand<TResponse>
        {
            MapHttpCommand<TCommand, TResponse>(pattern, HttpMethods.Post);
        }

        /// <summary>
        /// Maps a POST command without response.
        /// </summary>
        public void MapPostCommand<TCommand>([StringSyntax("Route")] string pattern)
            where TCommand : IBonCommand
        {
            MapHttpCommand<TCommand>(pattern, HttpMethods.Post);
        }

        /// <summary>
        /// Maps a PUT command with response.
        /// </summary>
        public void MapPutCommand<TCommand, TResponse>([StringSyntax("Route")] string pattern)
            where TCommand : IBonCommand<TResponse>
        {
           MapHttpCommand<TCommand, TResponse>(pattern, HttpMethods.Put);
        }

        /// <summary>
        /// Maps a PUT command without response.
        /// </summary>
        public void MapPutCommand<TCommand>([StringSyntax("Route")] string pattern)
            where TCommand : IBonCommand
        {
            MapHttpCommand<TCommand>(pattern, HttpMethods.Put);
        }

        /// <summary>
        /// Maps a DELETE command with response.
        /// </summary>
        public void MapDeleteCommand<TCommand, TResponse>([StringSyntax("Route")] string pattern)
            where TCommand : IBonCommand<TResponse>
        {
            MapHttpCommand<TCommand, TResponse>(pattern, HttpMethods.Delete);
        }

        /// <summary>
        /// Maps a DELETE command without response.
        /// </summary>
        public void MapDeleteCommand<TCommand>([StringSyntax("Route")] string pattern)
            where TCommand : IBonCommand
        {
            MapHttpCommand<TCommand>(pattern, HttpMethods.Delete);
        }

        #endregion

        #region Query Methods

        /// <summary>
        /// Maps a GET query with response.
        /// </summary>
        public void MapQuery<TQuery, TResponse>([StringSyntax("Route")] string pattern)
            where TQuery : IBonQuery<TResponse>
        {
            _nonEndpointBuilder.MapGet(pattern, async (HttpContext context) =>
            {
                var query = await context.Request.ReadFromJsonAsync<TQuery>();
                var mediator = context.RequestServices.GetRequiredService<IBonMediator>();
                var response = await mediator.QueryAsync<TQuery, TResponse>(query, context.RequestAborted);
                return Results.Ok(response);
            })
            .Accepts<TQuery>("application/json")
            .Produces<TResponse>();
        }

        #endregion

        #region Event Methods

        /// <summary>
        /// Maps a POST event.
        /// </summary>
        public void MapEvent<TEvent>([StringSyntax("Route")] string pattern)
            where TEvent : IBonEvent
        {
            _nonEndpointBuilder.MapPost(pattern, async (HttpContext context) =>
            {
                var eventObj = await context.Request.ReadFromJsonAsync<TEvent>();
                var mediator = context.RequestServices.GetRequiredService<IBonMediator>();
                await mediator.PublishAsync(eventObj, context.RequestAborted);
                return Results.Ok();
            })
            .Accepts<TEvent>("application/json");
        }

        #endregion

        #region Helper Methods

        // Helper method to handle POST/PUT/DELETE with or without response
        private void MapHttpCommand<TCommand>(string pattern, string httpMethod)
            where TCommand : IBonCommand
        {
            _nonEndpointBuilder.MapMethods(pattern,new[] { httpMethod }, async (HttpContext context) =>
            {
                var command = await context.Request.ReadFromJsonAsync<TCommand>();
                var mediator = context.RequestServices.GetRequiredService<IBonMediator>();
                await mediator.SendAsync(command, context.RequestAborted);
                return Results.Ok();
            })
            .Accepts<TCommand>("application/json");
        }

        // Helper method for commands with response
        private void MapHttpCommand<TCommand, TResponse>(string pattern, string httpMethod)
            where TCommand : IBonCommand<TResponse>
        {
            _nonEndpointBuilder.MapMethods(pattern,new[] { httpMethod },  async (HttpContext context) =>
            {
                var command = await context.Request.ReadFromJsonAsync<TCommand>();
                var mediator = context.RequestServices.GetRequiredService<IBonMediator>();
                var response = await mediator.SendAsync<TCommand, TResponse>(command, context.RequestAborted);
                return Results.Ok(response);
            })
            .Accepts<TCommand>("application/json")
            .Produces<TResponse>();
        }
        
        

        #endregion
    }
}
