using Bonyan.Layer.Domain.ValueObjects;

namespace Bonyan.IdentityManagement.Domain.Permissions.ValueObjects;

public class BonPermissionId : BonBusinessId<BonPermissionId, string> , IParsable<BonPermissionId>
{
    public static BonPermissionId Parse(string s, IFormatProvider? provider)
    {
        return BonPermissionId.FromValue(s);
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out BonPermissionId result)
    {
        result = Parse(s,provider);
        return true;
    }
}