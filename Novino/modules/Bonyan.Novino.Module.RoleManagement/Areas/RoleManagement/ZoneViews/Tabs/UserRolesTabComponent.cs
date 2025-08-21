using Bonyan.AspNetCore.ZoneComponent;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.IdentityManagement.Domain.Roles.DomainServices;
using Bonyan.IdentityManagement.Permissions;
using Bonyan.Novino.Module.RoleManagement.Areas.RoleManagement.Permissions;
using Bonyan.Novino.Module.RoleManagement.Areas.RoleManagement.ViewModels;
using Bonyan.Novino.Domain.Entities;
using Bonyan.Novino.Module.UserManagement.Abstractions.Zones.DetailsPage;
using Bonyan.UnitOfWork;
using Bonyan.User;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Bonyan.Novino.Module.RoleManagement.Areas.RoleManagement.ZoneViews.Tabs;

/// <summary>
/// Tab component for user roles
/// </summary>
public class UserRolesTabComponent : ZoneTabViewComponent<UserDetailPageTabZone>
{
    private readonly IBonIdentityUserRepository<Domain.Entities.User, Role> _userRepository;
    private readonly IBonIdentityUserManager<Domain.Entities.User, Role> _userManager;
    private readonly IBonIdentityRoleManager<Role> _roleManager;
    private readonly IBonPermissionManager<Domain.Entities.User, Role> _permissionManager;
    private readonly IBonCurrentUser _currentUser;
    private readonly IBonUnitOfWorkManager _unitOfWorkManager;
    private readonly ILogger<UserRolesTabComponent> _logger;

    public UserRolesTabComponent(
        IBonIdentityUserRepository<Domain.Entities.User, Role> userRepository,
        IBonIdentityUserManager<Domain.Entities.User, Role> userManager,
        IBonIdentityRoleManager<Role> roleManager,
        IBonPermissionManager<Domain.Entities.User, Role> permissionManager,
        IBonCurrentUser currentUser,
        IBonUnitOfWorkManager unitOfWorkManager,
        ILogger<UserRolesTabComponent> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _permissionManager = permissionManager ?? throw new ArgumentNullException(nameof(permissionManager));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        _unitOfWorkManager = unitOfWorkManager ?? throw new ArgumentNullException(nameof(unitOfWorkManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Custom unique ID to prevent duplicates
    /// </summary>
    public override string Id => "user_roles_tab";

    public override int Priority => 200;

    /// <summary>
    /// Default tab parameters for user roles tab
    /// </summary>
    protected override TabParameters DefaultTabParameters => new TabParameters(
        tabId: "user-roles",
        tabText: "نقش‌ها",
        tabIcon: "ri-shield-user-line",
        isActive: false
    );

    public override async Task<TabContentResult> HandleTabAsync(UserDetailPageTabZone model, ZoneRenderingContext context, ZoneComponentParameters parameters, CancellationToken cancellationToken = default)
    {
        var logger = context.GetServiceOrDefault<IZoneComponentLogger>();
        logger?.LogInformation(Id, $"Rendering user roles tab for user: {model.UserId ?? "Unknown"}");

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Check if current user has permission to view roles
            var currentUserId = BonUserId.NewId(_currentUser.Id!.Value);
            var canViewRoles = await _permissionManager.HasPermissionAsync(currentUserId, RoleManagementPermissions.Roles.ViewUsers);
            var canManageRoles = await _permissionManager.HasPermissionAsync(currentUserId, RoleManagementPermissions.Roles.ManageUsers);

            if (!canViewRoles)
            {
                _logger.LogInformation("User {UserId} does not have permission to view roles", model.UserId);
                return CreateRegularContentResult("");
            }

            // Get user data from repository
            var userId = BonUserId.FromValue(Guid.Parse(model.UserId));
            var userResult = await _userManager.FindByIdAsync(userId);
            
            if (!userResult.IsSuccess)
            {
                _logger.LogWarning("User not found for ID: {UserId}", model.UserId);
                return CreateTabResult("<div class='alert alert-warning'>کاربر یافت نشد</div>");
            }

            var user = userResult.Value;
            
            // Get user roles using role manager
            var rolesresult = await _userManager.GetUserRolesAsync(user);

            // Get available roles for assignment
            var availableRolesResult = await _roleManager.GetAllRolesAsync();
            var availableRoles = availableRolesResult.IsSuccess ? availableRolesResult.Value.ToList() : new List<Role>();

            // Create role-specific user details view model
            var userRoleDetails = new UserRoleDetailsViewModel
            {
                UserId = user.Id.ToString(),
                UserName = user.UserName,
                FullName = $"{user.Profile.FirstName} {user.Profile.LastName}".Trim(),
                Email = user.Email?.Address ?? "",
                PhoneNumber = user.PhoneNumber?.Number,
                IsActive = user.Status == UserStatus.Active,
                IsLocked = user.Status == UserStatus.Locked,
                CreatedAt = user.CreatedAt,
                LastLoginAt = null, // This would need to be implemented based on your login tracking
                CanViewRoles = canViewRoles,
                CanManageRoles = canManageRoles,
                CanAssignRoles = await _permissionManager.HasPermissionAsync(currentUserId, RoleManagementPermissions.Roles.AssignToUser),
                CanRemoveRoles = await _permissionManager.HasPermissionAsync(currentUserId, RoleManagementPermissions.Roles.RemoveFromUser),
                CanViewRoleDetails = await _permissionManager.HasPermissionAsync(currentUserId, RoleManagementPermissions.Roles.Details),
                CurrentUserId = _currentUser.Id?.ToString() ?? string.Empty,
                CurrentUserName = _currentUser.UserName ?? string.Empty,
                Roles = rolesresult.IsSuccess ? rolesresult.Value.Select(r => new UserRoleViewModel
                {
                    RoleId = r.Id.ToString(),
                    RoleName = r.Title,
                    DisplayName = r.Title,
                    AssignedAt = DateTime.Now, // This should come from user-role relationship
                    AssignedBy = "System", // This should come from audit data
                    ClaimsCount = r.RoleClaims?.Count ?? 0,
                    CanRemove = canManageRoles,
                    CanViewDetails = canViewRoles,
                    IsActive = true // This should come from role status
                }).ToList() : new List<UserRoleViewModel>(),
                AvailableRoles = availableRoles.Select(r => new AvailableRoleViewModel
                {
                    RoleId = r.Id.ToString(),
                    RoleName = r.Title,
                    DisplayName = r.Title,
                    UsersCount = 0, // This should come from role-user relationship count
                    ClaimsCount = r.RoleClaims?.Count ?? 0,
                    IsAlreadyAssigned = rolesresult.IsSuccess && rolesresult.Value.Any(ur => ur.Id == r.Id),
                    CanAssign = canManageRoles,
                    CanViewDetails = canViewRoles
                }).ToList()
            };

            // Update tab parameters based on roles count
            TabParameters.BadgeText = userRoleDetails.RolesCount.ToString();
            TabParameters.Tooltip = $"تعداد نقش‌ها: {userRoleDetails.RolesCount}";

            // Render the roles partial view
            logger?.LogInformation(Id, $"About to render _UserRolesPartial with model: UserId={userRoleDetails.UserId}, RolesCount={userRoleDetails.RolesCount}, Roles.Count={userRoleDetails.Roles?.Count ?? 0}");
            
            var rolesHtml = await context.RenderPartialViewAsync("Shared/_UserRolesPartial", userRoleDetails, cancellationToken);
            
            logger?.LogInformation(Id, $"Successfully rendered user roles tab with {userRoleDetails.RolesCount} roles");

            return CreateTabResult(rolesHtml);
        }
        catch (OperationCanceledException)
        {
            logger?.LogWarning(Id, "User roles tab rendering was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            logger?.LogError(Id, "Error rendering user roles tab", ex);
            throw;
        }
    }
} 