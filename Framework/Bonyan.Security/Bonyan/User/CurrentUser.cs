using System.Security.Claims;
using System.Security.Principal.System.Security;
using Bonyan.Security.Claims;

namespace Bonyan.User;

public class CurrentUser : ICurrentUser
{
    private static readonly Claim[] EmptyClaimsArray = new Claim[0];

    public virtual bool IsAuthenticated => Id.HasValue;

    public virtual Guid? Id => _principalAccessor.Principal?.FindUserId();

    public virtual string? UserName => this.FindClaimValue(BonyanClaimTypes.UserName);

    public virtual string? Name => this.FindClaimValue(BonyanClaimTypes.Name);

    public virtual string? SurName => this.FindClaimValue(BonyanClaimTypes.SurName);

    public virtual string? PhoneNumber => this.FindClaimValue(BonyanClaimTypes.PhoneNumber);

    public virtual bool PhoneNumberVerified => string.Equals(this.FindClaimValue(BonyanClaimTypes.PhoneNumberVerified), "true", StringComparison.InvariantCultureIgnoreCase);

    public virtual string? Email => this.FindClaimValue(BonyanClaimTypes.Email);

    public virtual bool EmailVerified => string.Equals(this.FindClaimValue(BonyanClaimTypes.EmailVerified), "true", StringComparison.InvariantCultureIgnoreCase);

    public virtual Guid? TenantId => _principalAccessor.Principal?.FindTenantId();

    public virtual string[] Roles => FindClaims(BonyanClaimTypes.Role).Select(c => c.Value).Distinct().ToArray();

    private readonly ICurrentPrincipalAccessor _principalAccessor;

    public CurrentUser(ICurrentPrincipalAccessor principalAccessor)
    {
        _principalAccessor = principalAccessor;
    }

    public virtual Claim? FindClaim(string claimType)
    {
        return _principalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == claimType);
    }

    public virtual Claim[] FindClaims(string claimType)
    {
        return _principalAccessor.Principal?.Claims.Where(c => c.Type == claimType).ToArray() ?? EmptyClaimsArray;
    }

    public virtual Claim[] GetAllClaims()
    {
        return _principalAccessor.Principal?.Claims.ToArray() ?? EmptyClaimsArray;
    }

    public virtual bool IsInRole(string roleName)
    {
        return FindClaims(BonyanClaimTypes.Role).Any(c => c.Value == roleName);
    }
}