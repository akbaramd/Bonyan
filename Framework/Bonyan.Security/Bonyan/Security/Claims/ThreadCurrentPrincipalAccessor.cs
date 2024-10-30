using System.Security.Claims;

namespace Bonyan.Security.Claims;

public class ThreadCurrentPrincipalAccessor : CurrentPrincipalAccessorBase
{
    protected override ClaimsPrincipal GetClaimsPrincipal()
    {
        return (Thread.CurrentPrincipal as ClaimsPrincipal)!;
    }
}
