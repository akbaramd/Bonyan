using System.Security.Claims;
using System.Security.Principal.System.Security;
using Bonyan.Security.Claims;

namespace Bonyan.User;

public class BonCurrentUser : IBonCurrentUser
{
    private static readonly Claim[] EmptyClaimsArray = new Claim[0];

    public virtual bool IsAuthenticated => Id.HasValue;

    public virtual Guid? Id => _principalAccessor.Principal?.FindUserId();

    public virtual string? UserName => this.FindClaimValue(BonClaimTypes.UserName);

    public virtual string? Name => this.FindClaimValue(BonClaimTypes.Name);

    public virtual string? SurName => this.FindClaimValue(BonClaimTypes.SurName);

    public virtual string? PhoneNumber => this.FindClaimValue(BonClaimTypes.PhoneNumber);

    public virtual bool PhoneNumberVerified => string.Equals(this.FindClaimValue(BonClaimTypes.PhoneNumberVerified), "true", StringComparison.InvariantCultureIgnoreCase);

    public virtual string? Email => this.FindClaimValue(BonClaimTypes.Email);

    public virtual bool EmailVerified => string.Equals(this.FindClaimValue(BonClaimTypes.EmailVerified), "true", StringComparison.InvariantCultureIgnoreCase);

    public virtual Guid? TenantId => _principalAccessor.Principal?.FindTenantId();

    public virtual string[] Roles => FindClaims(BonClaimTypes.Role).Select(c => c.Value).Distinct().ToArray();

    private readonly IBonCurrentPrincipalAccessor _principalAccessor;

    public BonCurrentUser(IBonCurrentPrincipalAccessor principalAccessor)
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
        return FindClaims(BonClaimTypes.Role).Any(c => c.Value == roleName);
    }
}