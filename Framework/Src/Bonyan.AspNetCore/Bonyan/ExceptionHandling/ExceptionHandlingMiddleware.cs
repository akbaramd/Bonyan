using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bonyan.ExceptionHandling;

/// <summary>
/// Middleware for handling exceptions and mapping them to appropriate HTTP responses.
/// Uses pluggable mappers and serializers following microkernel architecture.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IExceptionToHttpResultMapper _mapper;
    private readonly IExceptionResponseSerializer _serializer;
    private readonly ExceptionHandlingOptions _options;
    private readonly ILogger<ExceptionHandlingMiddleware>? _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        IExceptionToHttpResultMapper mapper,
        IExceptionResponseSerializer serializer,
        IOptions<ExceptionHandlingOptions> options,
        ILogger<ExceptionHandlingMiddleware>? logger = null)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unhandled exception in request pipeline for {Path}", context.Request.Path);

            // Map exception to HTTP result
            var result = _mapper.MapToHttpResult(ex, context);

            // Set status code
            context.Response.StatusCode = result.StatusCode;

            // Serialize and write response
            await _serializer.SerializeAsync(context, result.ResponseBody, context.RequestAborted);
        }
    }
}
