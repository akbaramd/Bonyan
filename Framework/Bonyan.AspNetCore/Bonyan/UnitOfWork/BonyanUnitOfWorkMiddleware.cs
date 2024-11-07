using Microsoft.Extensions.Options;

namespace Bonyan.UnitOfWork;

public class BonyanUnitOfWorkMiddleware : IMiddleware
{
    private readonly IBonUnitOfWorkManager _bonUnitOfWorkManager;
    private readonly BonyanAspNetCoreUnitOfWorkOptions _options;

    public BonyanUnitOfWorkMiddleware(
        IBonUnitOfWorkManager bonUnitOfWorkManager,
        IOptions<BonyanAspNetCoreUnitOfWorkOptions> options)
    {
        _bonUnitOfWorkManager = bonUnitOfWorkManager;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (IsIgnoredUrl(context))
        {
            await next(context);
            return;
        }

        using (var uow = _bonUnitOfWorkManager.Reserve(BonUnitOfWork.UnitOfWorkReservationName))
        {
            await next(context);
            await uow.CompleteAsync(context.RequestAborted);
        }
    }

    private bool IsIgnoredUrl(HttpContext context)
    {
        return context.Request.Path.Value != null &&
               _options.IgnoredUrls.Any(x => context.Request.Path.Value.StartsWith(x));
    }
}
