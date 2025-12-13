using System.Net;
using Bonyan.Layer.Application.Exceptions;
using Bonyan.Layer.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Bonyan.ExceptionHandling;

/// <summary>
/// Default implementation of exception to HTTP result mapping.
/// Maps exceptions to appropriate HTTP status codes following REST API best practices.
/// </summary>
public class DefaultExceptionToHttpResultMapper : IExceptionToHttpResultMapper
{
    /// <summary>
    /// Maps an exception to an HTTP result with appropriate status code.
    /// </summary>
    public ExceptionHttpResult MapToHttpResult(Exception exception, HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(exception);
        ArgumentNullException.ThrowIfNull(httpContext);

        return exception switch
        {
            // Domain exceptions - typically 400 Bad Request (client error)
            BonDomainException domainEx => new ExceptionHttpResult
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                ResponseBody = new HttpExceptionModel(
                    nameof(BonDomainException),
                    domainEx.ErrorCode,
                    domainEx.Message,
                    (int)HttpStatusCode.BadRequest,
                    domainEx.Parameters)
            },

            // Application exceptions - typically 400 or 422
            BonApplicationException appEx => new ExceptionHttpResult
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                ResponseBody = new HttpExceptionModel(
                    nameof(BonApplicationException),
                    appEx.ErrorCode,
                    appEx.Message,
                    (int)HttpStatusCode.BadRequest,
                    appEx.Parameters)
            },

            // Argument exceptions - 400 Bad Request
            ArgumentException argEx => new ExceptionHttpResult
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                ResponseBody = new HttpExceptionModel(
                    nameof(ArgumentException),
                    "INVALID_ARGUMENT",
                    argEx.Message,
                    (int)HttpStatusCode.BadRequest)
            },

            // Not found - 404
            KeyNotFoundException keyEx => new ExceptionHttpResult
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                ResponseBody = new HttpExceptionModel(
                    nameof(KeyNotFoundException),
                    "NOT_FOUND",
                    keyEx.Message,
                    (int)HttpStatusCode.NotFound)
            },

            // Unauthorized - 401
            UnauthorizedAccessException authEx => new ExceptionHttpResult
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
                ResponseBody = new HttpExceptionModel(
                    nameof(UnauthorizedAccessException),
                    "UNAUTHORIZED",
                    authEx.Message,
                    (int)HttpStatusCode.Unauthorized)
            },

            // Forbidden - 403
            InvalidOperationException when exception.Message.Contains("forbidden", StringComparison.OrdinalIgnoreCase) => new ExceptionHttpResult
            {
                StatusCode = (int)HttpStatusCode.Forbidden,
                ResponseBody = new HttpExceptionModel(
                    exception.GetType().Name,
                    "FORBIDDEN",
                    exception.Message,
                    (int)HttpStatusCode.Forbidden)
            },

            // Default - 500 Internal Server Error
            _ => new ExceptionHttpResult
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                ResponseBody = new HttpExceptionModel(
                    exception.GetType().Name,
                    "INTERNAL_ERROR",
                    exception.InnerException?.Message ?? exception.Message,
                    (int)HttpStatusCode.InternalServerError)
            }
        };
    }
}

