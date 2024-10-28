using System.Net;
using Bonyan.DomainDrivenDesign.Application.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SharedKernel.ExceptionHandling.Models;
using ApplicationException = Bonyan.DomainDrivenDesign.Application.Exceptions.ApplicationException;


namespace SharedKernel.ExceptionHandling.Middlewares;

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

      if (ex is ApplicationException exception)
      {
        var response = new HttpExceptionModel(nameof(ApplicationException),exception.ErrorCode, ex.Message,
          (int)HttpStatusCode.InternalServerError, exception.Parameters);

        await context.Response.WriteAsync(JsonConvert.SerializeObject(response,
          new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
      }
      else if (ex is DomainException domainException)
      {
          var response = new HttpExceptionModel(nameof(DomainException), domainException.ErrorCode,ex.Message, (int)HttpStatusCode.InternalServerError);
      
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
