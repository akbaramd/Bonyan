using Microsoft.Extensions.Options;

namespace Bonyan.UnitOfWork;

public class AspNetCoreUnitOfWorkTransactionBehaviourProvider : IUnitOfWorkTransactionBehaviourProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AspNetCoreUnitOfWorkTransactionBehaviourProviderOptions _options;

    public virtual bool? IsTransactional {
        get {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return null;
            }

            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                return null;
            }

            var currentUrl = httpContext.Request.Path.Value;
            if (currentUrl != null)
            {
                foreach (var url in _options.NonTransactionalUrls)
                {
                    if (currentUrl.StartsWith(url, StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }
            }

            return !string.Equals(
                httpContext.Request.Method,
                HttpMethod.Get.Method, StringComparison.OrdinalIgnoreCase
            );
        }
    }

    public AspNetCoreUnitOfWorkTransactionBehaviourProvider(
        IHttpContextAccessor httpContextAccessor,
        IOptions<AspNetCoreUnitOfWorkTransactionBehaviourProviderOptions> options)
    {
        _httpContextAccessor = httpContextAccessor;
        _options = options.Value;
    }
}
