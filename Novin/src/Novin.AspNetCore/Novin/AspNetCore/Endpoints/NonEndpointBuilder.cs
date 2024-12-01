using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Novin.AspNetCore.Novin.AspNetCore.Endpoints
{
    public class NonEndpointBuilder
    {
        private readonly WebApplication _app;

        public NonEndpointBuilder(WebApplication app)
        {
            _app = app ?? throw new ArgumentNullException(nameof(app));
        }

        public RouteHandlerBuilder Map([StringSyntax("Route")] string pattern, Delegate handler)
        {
            return _app.Map(pattern, handler);
        }

        public RouteHandlerBuilder MapMethods(string[] methods, [StringSyntax("Route")] string pattern, Delegate handler)
        {
            return _app.MapMethods(pattern, methods, handler);
        }

        // Map GET requests to a route with a delegate handler
        public RouteHandlerBuilder MapGet([StringSyntax("Route")] string pattern, Delegate handler)
        {
            return _app.MapGet(pattern, handler);
        }

        // Map POST requests to a route with a delegate handler
        public RouteHandlerBuilder MapPost([StringSyntax("Route")] string pattern, Delegate handler)
        {
            return _app.MapPost(pattern, handler);
        }

        // Map PUT requests to a route with a delegate handler
        public RouteHandlerBuilder MapPut([StringSyntax("Route")] string pattern, Delegate handler)
        {
            return _app.MapPut(pattern, handler);
        }

        // Map DELETE requests to a route with a delegate handler
        public RouteHandlerBuilder MapDelete([StringSyntax("Route")] string pattern, Delegate handler)
        {
            return _app.MapDelete(pattern, handler);
        }

        // Map PATCH requests to a route with a delegate handler
        public RouteHandlerBuilder MapPatch([StringSyntax("Route")] string pattern, Delegate handler)
        {
            return _app.MapPatch(pattern, handler);
        }

        // Generic Map Methods with request handlers

        // Map GET requests with a handler from DI for a generic request type
        public RouteHandlerBuilder MapGetRequest<TRequest, TResponse>([StringSyntax("Route")] string pattern)
        {
            return _app.MapGet(pattern, async (HttpContext context) =>
            {
                var request = await context.Request.ReadFromJsonAsync<TRequest>();
                var handler = context.RequestServices.GetRequiredService<IEndpointHandler<TRequest, TResponse>>();
                var response = await handler.HandleRequestAsync(request, context);
                return Results.Ok(response);
            })
            .Accepts<TRequest>("application/json")
            .Produces<TResponse>();
        }

        // Map POST requests with a handler from DI for a generic request type
        public RouteHandlerBuilder MapPostRequest<TRequest, TResponse>([StringSyntax("Route")] string pattern)
        {
            return _app.MapPost(pattern, async (HttpContext context) =>
            {
                var request = await context.Request.ReadFromJsonAsync<TRequest>();
                var handler = context.RequestServices.GetRequiredService<IEndpointHandler<TRequest, TResponse>>();
                var response = await handler.HandleRequestAsync(request, context);
                return Results.Ok(response);
            })
            .Accepts<TRequest>("application/json")
            .Produces<TResponse>();
        }

        // Map PUT requests with a handler from DI for a generic request type
        public RouteHandlerBuilder MapPutRequest<TRequest, TResponse>([StringSyntax("Route")] string pattern)
        {
            return _app.MapPut(pattern, async (HttpContext context) =>
            {
                var request = await context.Request.ReadFromJsonAsync<TRequest>();
                var handler = context.RequestServices.GetRequiredService<IEndpointHandler<TRequest, TResponse>>();
                var response = await handler.HandleRequestAsync(request, context);
                return Results.Ok(response);
            })
            .Accepts<TRequest>("application/json")
            .Produces<TResponse>();
        }

        // Map DELETE requests with a handler from DI for a generic request type
        public RouteHandlerBuilder MapDeleteRequest<TRequest, TResponse>([StringSyntax("Route")] string pattern)
        {
            return _app.MapDelete(pattern, async (HttpContext context) =>
            {
                var request = await context.Request.ReadFromJsonAsync<TRequest>();
                var handler = context.RequestServices.GetRequiredService<IEndpointHandler<TRequest, TResponse>>();
                var response = await handler.HandleRequestAsync(request, context);
                return Results.Ok(response);
            })
            .Accepts<TRequest>("application/json")
            .Produces<TResponse>();
        }

        // Map PATCH requests with a handler from DI for a generic request type
        public RouteHandlerBuilder MapPatchRequest<TRequest, TResponse>([StringSyntax("Route")] string pattern)
        {
            return _app.MapPatch(pattern, async (HttpContext context) =>
            {
                var request = await context.Request.ReadFromJsonAsync<TRequest>();
                var handler = context.RequestServices.GetRequiredService<IEndpointHandler<TRequest, TResponse>>();
                var response = await handler.HandleRequestAsync(request, context);
                return Results.Ok(response);
            })
            .Accepts<TRequest>("application/json")
            .Produces<TResponse>();
        }

        // Map POST requests with a handler from DI for a non-generic request type
        public RouteHandlerBuilder MapPostRequest<TRequest>([StringSyntax("Route")] string pattern)
        {
            return _app.MapPost(pattern, async (HttpContext context) =>
            {
                var request = await context.Request.ReadFromJsonAsync<TRequest>();
                var handler = context.RequestServices.GetRequiredService<IEndpointHandler<TRequest>>();
                await handler.HandleRequestAsync(request, context);
                return Results.Ok();
            })
            .Accepts<TRequest>("application/json");
        }

        // Map PUT requests with a handler from DI for a non-generic request type
        public RouteHandlerBuilder MapPutRequest<TRequest>([StringSyntax("Route")] string pattern)
        {
            return _app.MapPut(pattern, async (HttpContext context) =>
            {
                var request = await context.Request.ReadFromJsonAsync<TRequest>();
                var handler = context.RequestServices.GetRequiredService<IEndpointHandler<TRequest>>();
                await handler.HandleRequestAsync(request, context);
                return Results.Ok();
            })
            .Accepts<TRequest>("application/json");
        }

        // Map DELETE requests with a handler from DI for a non-generic request type
        public RouteHandlerBuilder MapDeleteRequest<TRequest>([StringSyntax("Route")] string pattern)
        {
            return _app.MapDelete(pattern, async (HttpContext context) =>
            {
                var request = await context.Request.ReadFromJsonAsync<TRequest>();
                var handler = context.RequestServices.GetRequiredService<IEndpointHandler<TRequest>>();
                await handler.HandleRequestAsync(request, context);
                return Results.Ok();
            })
            .Accepts<TRequest>("application/json");
        }

        // Map PATCH requests with a handler from DI for a non-generic request type
        public RouteHandlerBuilder MapPatchRequest<TRequest>([StringSyntax("Route")] string pattern)
        {
            return _app.MapPatch(pattern, async (HttpContext context) =>
            {
                var request = await context.Request.ReadFromJsonAsync<TRequest>();
                var handler = context.RequestServices.GetRequiredService<IEndpointHandler<TRequest>>();
                await handler.HandleRequestAsync(request, context);
                return Results.Ok();
            })
            .Accepts<TRequest>("application/json");
        }
    }
}
