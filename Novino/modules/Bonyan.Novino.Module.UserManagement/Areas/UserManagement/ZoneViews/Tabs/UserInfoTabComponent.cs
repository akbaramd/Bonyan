using Bonyan.AspNetCore.ZoneComponent;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.Novino.Module.UserManagement.Abstractions.Zones.DetailsPage;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Models;
using Bonyan.Ui.Novino.Core;
using Bonyan.UserManagement.Domain.Users;
using Bonyan.Novino.Domain.Entities;
using Microsoft.Extensions.Logging;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.ViewModels;
using Bonyan.UnitOfWork;
using Bonyan.User;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Bonyan.IdentityManagement.Permissions;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Permissions;

namespace Bonyan.Novino.Module.UserManagement.Areas.UserManagement.ZoneViews.Tabs;

/// <summary>
/// Tab component for user information
/// </summary>
public class UserInfoTabComponent : ZoneTabViewComponent<UserDetailPageTabZone>
{
    private readonly IBonIdentityUserRepository<Domain.Entities.User, Role> _userRepository;
    private readonly IBonIdentityUserManager<Domain.Entities.User, Role> _userManager;
    private readonly IBonCurrentUser _currentUser;
    private readonly IBonUnitOfWorkManager _unitOfWorkManager;
    private readonly ILogger<UserInfoTabComponent> _logger;
    private readonly IBonPermissionManager<Domain.Entities.User, Role> _permissionManager;

    public UserInfoTabComponent(
        IBonIdentityUserRepository<Domain.Entities.User, Role> userRepository,
        IBonIdentityUserManager<Domain.Entities.User, Role> userManager,
        IBonCurrentUser currentUser,
        IBonUnitOfWorkManager unitOfWorkManager,
        ILogger<UserInfoTabComponent> logger,
        IBonPermissionManager<Domain.Entities.User, Role> permissionManager)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        _unitOfWorkManager = unitOfWorkManager ?? throw new ArgumentNullException(nameof(unitOfWorkManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _permissionManager = permissionManager ?? throw new ArgumentNullException(nameof(permissionManager));
    }

    /// <summary>
    /// Custom unique ID to prevent duplicates
    /// </summary>
    public override string Id => "user_info_tab";

    public override int Priority => 100;

    /// <summary>
    /// Default tab parameters for user info tab
    /// </summary>
    protected override TabParameters DefaultTabParameters => new TabParameters(
        tabId: "user-info",
        tabText: "اطلاعات کاربر",
        tabIcon: "ri-user-settings-line",
        isActive: true
    );

    public override async Task<TabContentResult> HandleTabAsync(UserDetailPageTabZone model, ZoneRenderingContext context, ZoneComponentParameters parameters, CancellationToken cancellationToken = default)
    {
        var logger = context.GetServiceOrDefault<IZoneComponentLogger>();
        logger?.LogInformation(Id, $"Rendering user info tab for user: {model.UserId ?? "Unknown"}");

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Check if current user has permission to view user information
            var currentUserId = BonUserId.NewId(_currentUser.Id!.Value);
            var canViewUserInfo = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.View);
            var canEditUser = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.Edit);
            var canDeleteUser = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.Delete);

            if (!canViewUserInfo)
            {
                _logger.LogInformation("User {UserId} does not have permission to view user information", model.UserId);
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

            // Get user roles and claims for statistics
            var rolesResult = await _userManager.GetUserRolesAsync(user);
            var claimsResult = await _userManager.GetAllClaimsAsync(user);

            // Create user details view model
            var userDetails = new UserDetailsViewModel
            {
                Id = user.Id.ToString(),
                UserName = user.UserName,
                Email = user.Email?.Address ?? "",
                Name = user.Profile.FirstName,
                SurName = user.Profile.LastName,
                PhoneNumber = user.PhoneNumber?.Number,
                NationalCode = user.Profile.NationalCode,
                DateOfBirth = user.Profile.DateOfBirth?.ToString("yyyy/MM/dd"),
                IsActive = user.Status == UserStatus.Active,
                IsLocked = user.Status == UserStatus.Locked,
                CreatedAt = user.CreatedAt,
                LastLoginAt = null, // This would need to be implemented based on your login tracking
                EmailConfirmed = user.Email?.IsVerified ?? false,
                PhoneNumberConfirmed = user.PhoneNumber?.IsVerified ?? false,
                FailedLoginAttempts = user.FailedLoginAttemptCount,
                AccountLockedUntil = user.AccountLockedUntil,
                BannedUntil = user.BannedUntil,
                CanBeDeleted = user.CanBeDeleted,
                CanEdit = canEditUser,
                CanDelete = canDeleteUser,
                CanChangePassword = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.ChangePassword),
                Claims = claimsResult.IsSuccess ? claimsResult.Value.Select(c => new UserClaimViewModel
                {
                    Id = c.Id.ToString(),
                    ClaimType = c.ClaimType,
                    ClaimValue = c.ClaimValue,
                    Issuer = c.Issuer,
                }).ToList() : new List<UserClaimViewModel>()
            };

          
            context.AddMeta("userDetails", userDetails);

            // Render user info partial view
            var userInfoHtml = await context.RenderPartialViewAsync("Shared/_UserInfoPartial", userDetails, cancellationToken);
            
            logger?.LogInformation(Id, "Successfully rendered user info tab");

            return CreateTabResult(userInfoHtml);
        }
        catch (OperationCanceledException)
        {
            logger?.LogWarning(Id, "User info tab rendering was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            logger?.LogError(Id, "Error rendering user info tab", ex);
            throw;
        }
    }
} 