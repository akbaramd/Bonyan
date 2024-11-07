using System.Diagnostics;
using Bonyan.Security.Claims;
using JetBrains.Annotations;

namespace Bonyan.User;

public static class BonCurrentUserExtensions
{
    public static string? FindClaimValue(this IBonCurrentUser bonCurrentUser, string claimType)
    {
        return bonCurrentUser.FindClaim(claimType)?.Value;
    }

    public static T FindClaimValue<T>(this IBonCurrentUser bonCurrentUser, string claimType)
        where T : struct
    {
        var value = bonCurrentUser.FindClaimValue(claimType);
        if (value == null)
        {
            return default;
        }

        return value.To<T>();
    }

    public static Guid GetId(this IBonCurrentUser bonCurrentUser)
    {
        Debug.Assert(bonCurrentUser.Id != null, "currentUser.Id != null");

        return bonCurrentUser!.Id!.Value;
    }

    public static Guid? FindImpersonatorTenantId([NotNull] this IBonCurrentUser bonCurrentUser)
    {
        var impersonatorTenantId = bonCurrentUser.FindClaimValue(BonClaimTypes.ImpersonatorTenantId);
        if (impersonatorTenantId.IsNullOrWhiteSpace())
        {
            return null;
        }
        if (Guid.TryParse(impersonatorTenantId, out var guid))
        {
            return guid;
        }

        return null;
    }

    public static Guid? FindImpersonatorUserId([NotNull] this IBonCurrentUser bonCurrentUser)
    {
        var impersonatorUserId = bonCurrentUser.FindClaimValue(BonClaimTypes.ImpersonatorUserId);
        if (impersonatorUserId.IsNullOrWhiteSpace())
        {
            return null;
        }
        if (Guid.TryParse(impersonatorUserId, out var guid))
        {
            return guid;
        }

        return null;
    }

    public static string? FindImpersonatorTenantName([NotNull] this IBonCurrentUser bonCurrentUser)
    {
        return bonCurrentUser.FindClaimValue(BonClaimTypes.ImpersonatorTenantName);
    }

    public static string? FindImpersonatorUserName([NotNull] this IBonCurrentUser bonCurrentUser)
    {
        return bonCurrentUser.FindClaimValue(BonClaimTypes.ImpersonatorUserName);
    }

    public static string GetSessionId([NotNull] this IBonCurrentUser bonCurrentUser)
    {
        var sessionId = bonCurrentUser.FindSessionId();
        Debug.Assert(sessionId != null, "sessionId != null");
        return sessionId!;
    }

    public static string? FindSessionId([NotNull] this IBonCurrentUser bonCurrentUser)
    {
        return bonCurrentUser.FindClaimValue(BonClaimTypes.SessionId);
    }
}