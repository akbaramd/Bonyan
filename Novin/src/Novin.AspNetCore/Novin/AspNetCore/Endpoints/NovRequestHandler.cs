using Microsoft.AspNetCore.Http;

    namespace Novin.AspNetCore.Novin.AspNetCore.Endpoints
    {
        public interface IEndpointHandler<TRequest, TResponse>
        {
            Task<TResponse> HandleRequestAsync(TRequest request,HttpContext context);
        }
        
        public interface IEndpointHandler< TRequest>
        {
            Task HandleRequestAsync(TRequest request,HttpContext context);
        }
    }