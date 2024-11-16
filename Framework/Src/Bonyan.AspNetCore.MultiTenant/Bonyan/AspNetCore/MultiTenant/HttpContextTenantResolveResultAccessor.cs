using Bonyan.MultiTenant;

namespace Bonyan.AspNetCore.MultiTenant;

public class HttpContextTenantResolveResultAccessor : ITenantResolveResultAccessor
{
  public const string HttpContextItemName = "__TenantResolveResult";

  public TenantResolveResult? Result {
    get => _httpContextAccessor.HttpContext?.Items[HttpContextItemName] as TenantResolveResult;
    set {
      if (_httpContextAccessor.HttpContext == null)
      {
        return;
      }

      _httpContextAccessor.HttpContext.Items[HttpContextItemName] = value;
    }
  }

  private readonly IHttpContextAccessor _httpContextAccessor;

  public HttpContextTenantResolveResultAccessor(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }
}
