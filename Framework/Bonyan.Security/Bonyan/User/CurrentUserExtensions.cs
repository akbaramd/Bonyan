using System.Diagnostics;
using Bonyan.Security.Claims;
using JetBrains.Annotations;

namespace Bonyan.User;

public static class CurrentUserExtensions
{
    public static string? FindClaimValue(this ICurrentUser currentUser, string claimType)
    {
        return currentUser.FindClaim(claimType)?.Value;
    }

    public static T FindClaimValue<T>(this ICurrentUser currentUser, string claimType)
        where T : struct
    {
        var value = currentUser.FindClaimValue(claimType);
        if (value == null)
        {
            return default;
        }

        return value.To<T>();
    }

    public static Guid GetId(this ICurrentUser currentUser)
    {
        Debug.Assert(currentUser.Id != null, "currentUser.Id != null");

        return currentUser!.Id!.Value;
    }

    public static Guid? FindImpersonatorTenantId([NotNull] this ICurrentUser currentUser)
    {
        var impersonatorTenantId = currentUser.FindClaimValue(BonyanClaimTypes.ImpersonatorTenantId);
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

    public static Guid? FindImpersonatorUserId([NotNull] this ICurrentUser currentUser)
    {
        var impersonatorUserId = currentUser.FindClaimValue(BonyanClaimTypes.ImpersonatorUserId);
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

    public static string? FindImpersonatorTenantName([NotNull] this ICurrentUser currentUser)
    {
        return currentUser.FindClaimValue(BonyanClaimTypes.ImpersonatorTenantName);
    }

    public static string? FindImpersonatorUserName([NotNull] this ICurrentUser currentUser)
    {
        return currentUser.FindClaimValue(BonyanClaimTypes.ImpersonatorUserName);
    }

    public static string GetSessionId([NotNull] this ICurrentUser currentUser)
    {
        var sessionId = currentUser.FindSessionId();
        Debug.Assert(sessionId != null, "sessionId != null");
        return sessionId!;
    }

    public static string? FindSessionId([NotNull] this ICurrentUser currentUser)
    {
        return currentUser.FindClaimValue(BonyanClaimTypes.SessionId);
    }
}