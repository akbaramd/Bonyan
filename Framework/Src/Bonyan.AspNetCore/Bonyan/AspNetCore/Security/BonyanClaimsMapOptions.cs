using Bonyan.Security.Claims;

namespace Bonyan.AspNetCore.Security;

public class BonyanClaimsMapOptions
{
    public Dictionary<string, Func<string>> Maps { get; }

    public BonyanClaimsMapOptions()
    {
        Maps = new Dictionary<string, Func<string>>()
            {
                { "sub", () => BonClaimTypes.UserId },
                { "role", () => BonClaimTypes.Role },
                { "email", () => BonClaimTypes.Email },
                { "name", () => BonClaimTypes.UserName },
                { "family_name", () => BonClaimTypes.SurName },
                { "given_name", () => BonClaimTypes.Name }
            };
    }
}
