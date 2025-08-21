using System.Security.Claims;
using Bonyan.AspNetCore.ZoneComponent;
using Bonyan.IdentityManagement.Domain.Users;
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

namespace Bonyan.Novino.Module.UserManagement.Areas.UserManagement.ZoneViews.Tabs;

/// <summary>
/// Tab component for user claims
/// </summary>
public class UserClaimsTabComponent : ZoneTabViewComponent<UserDetailPageTabZone>
{
    private readonly IBonIdentityUserRepository<Domain.Entities.User, Role> _userRepository;
    private readonly IBonIdentityUserManager<Domain.Entities.User, Role> _userManager;
    private readonly IBonCurrentUser _currentUser;
    private readonly IBonUnitOfWorkManager _unitOfWorkManager;
    private readonly ILogger<UserClaimsTabComponent> _logger;
    private readonly IBonPermissionManager<Domain.Entities.User, Role> _permissionManager;

    public UserClaimsTabComponent(
        IBonIdentityUserRepository<Domain.Entities.User, Role> userRepository,
        IBonIdentityUserManager<Domain.Entities.User, Role> userManager,
        IBonCurrentUser currentUser,
        IBonUnitOfWorkManager unitOfWorkManager,
        ILogger<UserClaimsTabComponent> logger,
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
    public override string Id => "user_claims_tab";

    public override int Priority => 300;

    /// <summary>
    /// Default tab parameters for user claims tab
    /// </summary>
    protected override TabParameters DefaultTabParameters => new TabParameters(
        tabId: "user-claims",
        tabText: "مجوزها",
        tabIcon: "ri-file-list-line",
        isActive: false
    );

    public override async Task<TabContentResult> HandleTabAsync(UserDetailPageTabZone model, ZoneRenderingContext context, ZoneComponentParameters parameters, CancellationToken cancellationToken = default)
    {
        var logger = context.GetServiceOrDefault<IZoneComponentLogger>();
        logger?.LogInformation(Id, $"Rendering user claims tab for user: {model.UserId ?? "Unknown"}");

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Check if current user has permission to view claims
            var currentUserId = BonUserId.NewId(_currentUser.Id!.Value);
            var canViewClaims = await _permissionManager.HasPermissionAsync(currentUserId, BonIdentityManagementPermissions.Claims.ViewClaims);
            var canManageClaims = await _permissionManager.HasPermissionAsync(currentUserId, BonIdentityManagementPermissions.Users.ManageClaims);

            if (!canViewClaims)
            {
                _logger.LogInformation("User {UserId} does not have permission to view claims", model.UserId);
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

            // Get user claims
            var claimsResult = await _userManager.GetAllClaimsAsync(user);
            var claims = claimsResult.IsSuccess ? claimsResult.Value : new List<BonIdentityUserClaims<Domain.Entities.User,Role>>();

            // Create user details view model with claims
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
                LastLoginAt = null,
                EmailConfirmed = user.Email?.IsVerified ?? false,
                PhoneNumberConfirmed = user.PhoneNumber?.IsVerified ?? false,
                FailedLoginAttempts = user.FailedLoginAttemptCount,
                AccountLockedUntil = user.AccountLockedUntil,
                BannedUntil = user.BannedUntil,
                CanBeDeleted = user.CanBeDeleted,
                CanEdit = false, // Not needed for claims tab
                CanDelete = false, // Not needed for claims tab
                CanChangePassword = false, // Not needed for claims tab
                CanManageClaims = canManageClaims,
                Claims = claims.Select(c => new UserClaimViewModel
                {
                    Id = c.Id.ToString(),
                    ClaimType = c.ClaimType,
                    ClaimValue = c.ClaimValue,
                    Issuer = c.Issuer,
                }).ToList()
            };

            // Add metadata to zone context for other components
            context.AddMeta("canViewClaims", canViewClaims);
            context.AddMeta("canManageClaims", canManageClaims);
            context.AddMeta("userDetails", userDetails);

            // Update tab parameters based on claims count
            TabParameters.BadgeText = claims.Count().ToString();
            TabParameters.Tooltip = $"{claims.Count()} مجوز";

            // Render user claims partial view
            var claimsHtml = await context.RenderPartialViewAsync("Shared/_UserClaimsPartial", userDetails, cancellationToken);
            
            logger?.LogInformation(Id, $"Successfully rendered user claims tab with {claims.Count()} claims");

            return CreateTabResult(claimsHtml);
        }
        catch (OperationCanceledException)
        {
            logger?.LogWarning(Id, "User claims tab rendering was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            logger?.LogError(Id, "Error rendering user claims tab", ex);
            throw;
        }
    }
} 