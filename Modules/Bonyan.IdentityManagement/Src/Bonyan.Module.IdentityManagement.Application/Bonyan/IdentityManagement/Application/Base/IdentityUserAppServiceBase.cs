using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.Layer.Application.Services;
using Bonyan.Layer.Domain.DomainService;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Application.Base;

/// <summary>
/// Base for identity user application services. Provides user lookup and domain-result mapping.
/// User CRUD (list/detail/create/update/delete) stays in UserManagement; this base is for identity-only operations.
/// </summary>
public abstract class IdentityUserAppServiceBase : BonApplicationService
{
    protected IBonIdentityUserManager UserManager => LazyServiceProvider.LazyGetRequiredService<IBonIdentityUserManager>();

    /// <summary>
    /// Gets the user by id or returns a failed service result. Use in operations that require an existing user.
    /// </summary>
    protected async Task<(BonIdentityUser? User, ServiceResult? Fail)> GetUserOrFailAsync(BonUserId userId)
    {
        var result = await UserManager.FindByIdAsync(userId);
        if (result.IsFailure || result.Value == null)
            return (null, ServiceResult.Failure("User not found.", "NotFound"));
        return (result.Value, null);
    }

    protected static ServiceResult ToServiceResult(BonDomainResult domainResult) =>
        domainResult.IsSuccess
            ? ServiceResult.Success()
            : ServiceResult.Failure(domainResult.ErrorMessage ?? "Operation failed.", "DomainError");
}
