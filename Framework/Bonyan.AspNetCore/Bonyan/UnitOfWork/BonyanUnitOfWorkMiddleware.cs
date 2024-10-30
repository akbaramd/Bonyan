using Microsoft.Extensions.Options;

namespace Bonyan.UnitOfWork;

public class BonyanUnitOfWorkMiddleware : IMiddleware
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly BonyanAspNetCoreUnitOfWorkOptions _options;

    public BonyanUnitOfWorkMiddleware(
        IUnitOfWorkManager unitOfWorkManager,
        IOptions<BonyanAspNetCoreUnitOfWorkOptions> options)
    {
        _unitOfWorkManager = unitOfWorkManager;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (IsIgnoredUrl(context))
        {
            await next(context);
            return;
        }

        using (var uow = _unitOfWorkManager.Reserve(UnitOfWork.UnitOfWorkReservationName))
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
