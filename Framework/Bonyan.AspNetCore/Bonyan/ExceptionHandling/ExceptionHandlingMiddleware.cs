using System.Net;
using Bonyan.Layer.Domain.Exceptions;
using Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace Bonyan.ExceptionHandling;

public class ExceptionHandlingMiddleware
{
  private readonly RequestDelegate _next;

  public ExceptionHandlingMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (Exception ex)
    {
      context.Response.ContentType = "application/json";
      context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

      if (ex is BonApplicationException exception)
      {
        var response = new HttpExceptionModel(nameof(BonApplicationException),exception.ErrorCode, ex.Message,
          (int)HttpStatusCode.InternalServerError, exception.Parameters);

        await context.Response.WriteAsync(JsonConvert.SerializeObject(response,
          new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
      }
      else if (ex is BonDomainException domainException)
      {
          var response = new HttpExceptionModel(nameof(BonDomainException), domainException.ErrorCode,ex.Message, (int)HttpStatusCode.InternalServerError);
      
          await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings
          {
              ContractResolver = new CamelCasePropertyNamesContractResolver()
          }));
      }
      else
      {
        var response = new HttpExceptionModel(ex.GetType().Name,"GLOBAl", ex.InnerException?.Message ?? ex.Message,
          (int)HttpStatusCode.InternalServerError);
        await context.Response.WriteAsync(JsonConvert.SerializeObject(response,
          new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
      }
    }
  }
}
