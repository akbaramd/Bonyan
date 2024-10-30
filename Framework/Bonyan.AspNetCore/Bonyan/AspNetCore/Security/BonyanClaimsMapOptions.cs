using Bonyan.Security.Claims;

namespace Bonyan.AspNetCore.Security;

public class BonyanClaimsMapOptions
{
    public Dictionary<string, Func<string>> Maps { get; }

    public BonyanClaimsMapOptions()
    {
        Maps = new Dictionary<string, Func<string>>()
            {
                { "sub", () => BonyanClaimTypes.UserId },
                { "role", () => BonyanClaimTypes.Role },
                { "email", () => BonyanClaimTypes.Email },
                { "name", () => BonyanClaimTypes.UserName },
                { "family_name", () => BonyanClaimTypes.SurName },
                { "given_name", () => BonyanClaimTypes.Name }
            };
    }
}
