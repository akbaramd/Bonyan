using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bonyan.UnitOfWork;

/// <summary>
/// Middleware that manages Unit of Work lifecycle for HTTP requests.
/// Ensures proper transaction commit/rollback on success/failure.
/// </summary>
public class BonyanUnitOfWorkMiddleware : IMiddleware
{
    private readonly IBonUnitOfWorkManager _bonUnitOfWorkManager;
    private readonly BonyanAspNetCoreUnitOfWorkOptions _options;
    private readonly ILogger<BonyanUnitOfWorkMiddleware>? _logger;

    public BonyanUnitOfWorkMiddleware(
        IBonUnitOfWorkManager bonUnitOfWorkManager,
        IOptions<BonyanAspNetCoreUnitOfWorkOptions> options,
        ILogger<BonyanUnitOfWorkMiddleware>? logger = null)
    {
        _bonUnitOfWorkManager = bonUnitOfWorkManager ?? throw new ArgumentNullException(nameof(bonUnitOfWorkManager));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        // Skip UoW for ignored URLs
        if (IsIgnoredUrl(context))
        {
            await next(context);
            return;
        }

        using var uow = _bonUnitOfWorkManager.Reserve(BonUnitOfWork.UnitOfWorkReservationName);
        
        try
        {
            await next(context);
            
            // Only commit if request completed successfully (no exception)
            if (context.Response.StatusCode < 400)
            {
                _logger?.LogDebug("Completing Unit of Work for {Path}", context.Request.Path);
                await uow.CompleteAsync(context.RequestAborted);
            }
            else
            {
                _logger?.LogWarning("Skipping Unit of Work commit due to error status {StatusCode} for {Path}",
                    context.Response.StatusCode, context.Request.Path);
                // Don't explicitly rollback on error status - let disposal handle it
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Exception in request pipeline, Unit of Work will be rolled back for {Path}",
                context.Request.Path);
            
            // Exception occurred - ensure rollback
            try
            {
                await uow.RollbackAsync(CancellationToken.None);
            }
            catch (Exception rollbackEx)
            {
                _logger?.LogError(rollbackEx, "Failed to rollback Unit of Work for {Path}", context.Request.Path);
            }
            
            throw; // Re-throw to let exception handling middleware process it
        }
    }

    private bool IsIgnoredUrl(HttpContext context)
    {
        var path = context.Request.Path.Value;
        if (string.IsNullOrEmpty(path))
            return false;

        return _options.IgnoredUrls.Any(ignored => 
            path.StartsWith(ignored, StringComparison.OrdinalIgnoreCase));
    }
}
