using Bonyan.AspNetCore.ZoneComponent;
using Bonyan.Novino.Module.UserManagement.Abstractions.Zones.IndexPage;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.ViewModels;
using Bonyan.Ui.Novino.Core;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.Novino.Domain.Entities;
using Microsoft.Extensions.Logging;
using Bonyan.UnitOfWork;
using Bonyan.User;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Bonyan.IdentityManagement.Permissions;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Permissions;

namespace Bonyan.Novino.Module.UserManagement.Areas.UserManagement.ZoneViews;

/// <summary>
/// Zone component that renders user action buttons
/// </summary>
public class UserTableActionDefaultZoneView : ZoneViewComponent<UserIndexPageTableActionZone>
{
    private readonly IBonIdentityUserRepository<Domain.Entities.User, Role> _userRepository;
    private readonly IBonIdentityUserManager<Domain.Entities.User, Role> _userManager;
    private readonly IBonCurrentUser _currentUser;
    private readonly IBonUnitOfWorkManager _unitOfWorkManager;
    private readonly IBonPermissionManager<Domain.Entities.User, Role> _permissionManager;
    private readonly ILogger<UserTableActionDefaultZoneView> _logger;

    public UserTableActionDefaultZoneView(
        IBonIdentityUserRepository<Domain.Entities.User, Role> userRepository,
        IBonIdentityUserManager<Domain.Entities.User, Role> userManager,
        IBonCurrentUser currentUser,
        IBonUnitOfWorkManager unitOfWorkManager,
        IBonPermissionManager<Domain.Entities.User, Role> permissionManager,
        ILogger<UserTableActionDefaultZoneView> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        _unitOfWorkManager = unitOfWorkManager ?? throw new ArgumentNullException(nameof(unitOfWorkManager));
        _permissionManager = permissionManager ?? throw new ArgumentNullException(nameof(permissionManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Custom unique ID to prevent duplicates
    /// </summary>
    public override string Id => "user_table_default_actions";

    public override int Priority => 100;

    public override async Task<ZoneComponentResult> HandleAsync(UserIndexPageTableActionZone model, ZoneRenderingContext context, ZoneComponentParameters parameters, CancellationToken cancellationToken = default)
    {
        var logger = context.GetServiceOrDefault<IZoneComponentLogger>();
        logger?.LogInformation(Id, $"Rendering actions for user: {model.UserId ?? "Unknown"}");

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Check if current user has permission to view users
            var currentUserId = BonUserId.NewId(_currentUser.Id!.Value);
            var canViewUsers = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.View);

            if (!canViewUsers)
            {
                _logger.LogInformation("User {UserId} does not have permission to view users", _currentUser.Id);
                return ZoneComponentResult.Html("", true);
            }

            // Validate UserId
            if (string.IsNullOrEmpty(model.UserId) || !Guid.TryParse(model.UserId, out _))
            {
                _logger.LogWarning("Invalid UserId provided: {UserId}", model.UserId);
                return ZoneComponentResult.Html("", true);
            }

            // Get user data from repository
            var userId = BonUserId.FromValue(Guid.Parse(model.UserId));
            var userResult = await _userManager.FindByIdAsync(userId);

            if (!userResult.IsSuccess)
            {
                _logger.LogWarning("User not found for ID: {UserId}", model.UserId);
                return ZoneComponentResult.Html("", true);
            }

            var user = userResult.Value;

            // Check permissions for current user
            var canEditUser = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.Edit);
            var canDeleteUser = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.Delete);
            var canChangePassword = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.ChangePassword);
            var canLockUser = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.Lock);
            var canUnlockUser = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.Unlock);

            var actions = new List<string>();

            // Details button
            if (canViewUsers)
            {
                actions.Add($@"
                    <a href=""/UserManagement/User/Detail/{user.Id}"" 
                        class=""btn btn-sm btn-soft-info"" 
                        data-bs-toggle=""tooltip"" 
                        data-bs-placement=""top"" 
                        title=""مشاهده جزئیات"">
                        <i class=""ri-eye-fill align-bottom""></i>
                    </a>");
            }

            // Edit button
            if (canEditUser)
            {
                actions.Add($@"
                    <a href=""/UserManagement/User/Edit/{user.Id}"" 
                        class=""btn btn-sm btn-soft-warning"" 
                        data-bs-toggle=""tooltip"" 
                        data-bs-placement=""top"" 
                        title=""ویرایش کاربر"">
                        <i class=""ri-pencil-fill align-bottom""></i>
                    </a>");
            }

            // Delete button
            if (canDeleteUser && user.CanBeDeleted)
            {
                actions.Add($@"
                    <a href=""/UserManagement/User/DeleteConfirm/{user.Id}"" 
                        class=""btn btn-sm btn-soft-danger"" 
                        data-bs-toggle=""tooltip"" 
                        data-bs-placement=""top"" 
                        title=""حذف کاربر"">
                        <i class=""ri-delete-bin-5-fill align-bottom""></i>
                    </a>");
            }

            // Change Password button
            if (canChangePassword)
            {
                actions.Add($@"
                    <a href=""/UserManagement/User/ChangePassword/{user.Id}"" 
                        class=""btn btn-sm btn-soft-secondary"" 
                        data-bs-toggle=""tooltip"" 
                        data-bs-placement=""top"" 
                        title=""تغییر رمز عبور"">
                        <i class=""ri-lock-password-line align-bottom""></i>
                    </a>");
            }

            // Lock/Unlock button
            if ((canLockUser && user.Status != UserStatus.Locked) || (canUnlockUser && user.Status == UserStatus.Locked))
            {
                var isLocked = user.Status == UserStatus.Locked;
                var lockTitle = isLocked ? "باز کردن قفل" : "قفل کردن";
                var lockIcon = isLocked ? "ri-lock-unlock-line" : "ri-lock-line";
                var lockClass = isLocked ? "btn-soft-success" : "btn-soft-warning";
                var action = isLocked ? "unlock" : "lock";
                
                actions.Add($@"
                    <a href=""/UserManagement/User/Lock/{user.Id}/{action}"" 
                        class=""btn btn-sm {lockClass}"" 
                        data-bs-toggle=""tooltip"" 
                        data-bs-placement=""top"" 
                        title=""{lockTitle}"">
                        <i class=""{lockIcon} align-bottom""></i>
                    </a>");
            }

            var actionsHtml = string.Join("", actions);

            logger?.LogInformation(Id, $"Successfully rendered actions for user: {user.UserName}");

            // Add metadata to zone context for other components
            context.AddMeta("userDetails", new { Id = user.Id.ToString(), UserName = user.UserName, Email = user.Email?.Address });
            context.AddMeta("canEdit", canEditUser);
            context.AddMeta("canDelete", canDeleteUser);
            context.AddMeta("canChangePassword", canChangePassword);

            return ZoneComponentResult.Html(actionsHtml, true);
        }
        catch (OperationCanceledException)
        {
            logger?.LogWarning(Id, "Actions rendering was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            logger?.LogError(Id, "Error rendering user table actions", ex);
            throw;
        }
    }
}

