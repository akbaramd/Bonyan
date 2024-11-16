using System.Security.Claims;

namespace Bonyan.Security.Claims;

public class ThreadBonCurrentPrincipalAccessor : BonCurrentPrincipalAccessorBase
{
    protected override ClaimsPrincipal GetClaimsPrincipal()
    {
        return (Thread.CurrentPrincipal as ClaimsPrincipal)!;
    }
}
