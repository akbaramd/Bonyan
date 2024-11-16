using System.Security.Claims;

namespace Bonyan.Security.Claims;

public interface IBonCurrentPrincipalAccessor
{
    ClaimsPrincipal Principal { get; }

    IDisposable Change(ClaimsPrincipal principal);
}