using Bonyan.AspNetCore.ZoneComponent;
using Bonyan.Novino.Module.UserManagement.Abstractions.Zones.DetailsPage;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Models;
using Bonyan.Ui.Novino.Core;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.Novino.Domain.Entities;
using Microsoft.Extensions.Logging;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.ViewModels;
using Bonyan.UnitOfWork;
using Bonyan.User;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
    using Bonyan.IdentityManagement.Permissions;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Permissions;

namespace Bonyan.Novino.Module.UserManagement.Areas.UserManagement.ZoneViews;

/// <summary>
/// Zone component that renders user detail page header
/// </summary>
public class UserDetailPageHeaderZoneView : ZoneViewComponent<UserDetailPageHeaderZone>
{
    private readonly IBonIdentityUserRepository<Domain.Entities.User, Role> _userRepository;
    private readonly IBonIdentityUserManager<Domain.Entities.User, Role> _userManager;
    private readonly IBonCurrentUser _currentUser;
    private readonly IBonUnitOfWorkManager _unitOfWorkManager;
    private readonly IBonPermissionManager<Domain.Entities.User, Role> _permissionManager;
    private readonly ILogger<UserDetailPageHeaderZoneView> _logger;

    public UserDetailPageHeaderZoneView(
        IBonIdentityUserRepository<Domain.Entities.User, Role> userRepository,
        IBonIdentityUserManager<Domain.Entities.User, Role> userManager,
        IBonCurrentUser currentUser,
        IBonUnitOfWorkManager unitOfWorkManager,
        IBonPermissionManager<Domain.Entities.User, Role> permissionManager,
        ILogger<UserDetailPageHeaderZoneView> logger)
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
    public override string Id => "user_detail_page_header";

    public override int Priority => 100;

    public override async Task<ZoneComponentResult> HandleAsync(UserDetailPageHeaderZone model, ZoneRenderingContext context, ZoneComponentParameters parameters, CancellationToken cancellationToken = default)
    {
        var logger = context.GetServiceOrDefault<IZoneComponentLogger>();
        logger?.LogInformation(Id, $"Rendering header for user: {model.UserId ?? "Unknown"}");

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Check if current user has permission to view user information
            var currentUserId = BonUserId.NewId(_currentUser.Id!.Value);
            var canViewUser = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.View);
            
            if (!canViewUser)
            {
                _logger.LogInformation("User {UserId} does not have permission to view user information", _currentUser.Id);
                return ZoneComponentResult.Html("<div class='alert alert-warning'>شما مجوز مشاهده اطلاعات کاربر را ندارید</div>", true);
            }

            // Validate UserId
            if (string.IsNullOrEmpty(model.UserId) || !Guid.TryParse(model.UserId, out _))
            {
                _logger.LogWarning("Invalid UserId provided: {UserId}", model.UserId);
                return ZoneComponentResult.Html("<div class='alert alert-warning'>شناسه کاربر نامعتبر است</div>", true);
            }

            // Get user data from repository
            var userId = BonUserId.FromValue(Guid.Parse(model.UserId));
            var userResult = await _userManager.FindByIdAsync(userId);
            
            if (!userResult.IsSuccess)
            {
                _logger.LogWarning("User not found for ID: {UserId}", model.UserId);
                return ZoneComponentResult.Html("<div class='alert alert-warning'>کاربر یافت نشد</div>", true);
            }

            var user = userResult.Value;
            
            // Check permissions for current user
            var canEditUser = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.Edit);
            var canDeleteUser = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.Delete);
            var canChangePassword = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.ChangePassword);

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
                CanChangePassword = canChangePassword,
                Claims = claimsResult.IsSuccess ? claimsResult.Value.Select(c => new UserClaimViewModel
                {
                    Id = c.Id.ToString(),
                    ClaimType = c.ClaimType,
                    ClaimValue = c.ClaimValue,
                    Issuer = c.Issuer,
                }).ToList() : new List<UserClaimViewModel>()
            };

            // Render the header HTML
            var headerHtml = $@"
                <div class=""row align-items-center"">
                    <div class=""col-auto"">
                        <div class=""avatar-lg"">
                            <div class=""avatar-title bg-primary-subtle text-primary rounded-circle fs-24"">
                                <i class=""ri-user-line""></i>
                            </div>
                        </div>
                    </div>
                    <div class=""col"">
                        <div class=""d-flex flex-column"">
                            <h4 class=""mb-1"">{userDetails.FullName}</h4>
                            <p class=""text-muted mb-2"">{userDetails.UserName}</p>
                            <div class=""d-flex gap-2 align-items-center"">
                                {GetStatusBadges(userDetails)}
                            </div>
                        </div>
                    </div>
                    <div class=""col-auto"">
                        <div class=""d-flex gap-2"">
                            {GetActionButtons(userDetails)}
                        </div>
                    </div>
                </div>";

            logger?.LogInformation(Id, $"Successfully rendered header for user: {userDetails.UserName}");

            // Add metadata to zone context for other components
            context.AddMeta("userDetails", userDetails);
            context.AddMeta("canEdit", canEditUser);
            context.AddMeta("canDelete", canDeleteUser);
            context.AddMeta("canChangePassword", canChangePassword);

            return ZoneComponentResult.Html(headerHtml, true);
        }
        catch (OperationCanceledException)
        {
            logger?.LogWarning(Id, "Header rendering was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            logger?.LogError(Id, "Error rendering user detail header", ex);
            throw;
        }
    }

    private string GetStatusBadges(UserDetailsViewModel user)
    {
        var badges = new List<string>();

        // Status badge
        if (user.IsActive)
        {
            badges.Add(@"<span class=""badge bg-success-subtle text-success"">
                <i class=""ri-checkbox-circle-line me-1""></i>فعال
            </span>");
        }
        else if (user.IsLocked)
        {
            badges.Add(@"<span class=""badge bg-warning-subtle text-warning"">
                <i class=""ri-lock-line me-1""></i>قفل شده
            </span>");
        }
        else
        {
            badges.Add(@"<span class=""badge bg-danger-subtle text-danger"">
                <i class=""ri-close-circle-line me-1""></i>غیرفعال
            </span>");
        }

        // Email confirmation badge
        if (user.EmailConfirmed)
        {
            badges.Add(@"<span class=""badge bg-info-subtle text-info"">
                <i class=""ri-mail-check-line me-1""></i>ایمیل تأیید شده
            </span>");
        }

        // Phone confirmation badge
        if (user.PhoneNumberConfirmed)
        {
            badges.Add(@"<span class=""badge bg-info-subtle text-info"">
                <i class=""ri-phone-check-line me-1""></i>تلفن تأیید شده
            </span>");
        }

        return string.Join("", badges);
    }

    private string GetActionButtons(UserDetailsViewModel user)
    {
        var buttons = new List<string>();

        // Edit button
        if (user.CanEdit)
        {
            buttons.Add($@"<a href=""/UserManagement/User/Edit/{user.Id}"" 
                class=""btn btn-primary"">
                <i class=""ri-edit-line align-bottom me-1""></i>ویرایش
            </a>");
        }

        // Change password button
        if (user.CanChangePassword)
        {
            buttons.Add(@"<button type=""button"" class=""btn btn-warning"" data-bs-toggle=""modal"" data-bs-target=""#changePasswordModal"">
                <i class=""ri-lock-password-line align-bottom me-1""></i>تغییر رمز
            </button>");
        }

        // Delete button
        if (user.CanDelete && user.CanBeDeleted)
        {
            buttons.Add($@"<button type=""button"" class=""btn btn-danger"" onclick=""deleteUser('{user.Id}', '{user.UserName}')"">
                <i class=""ri-delete-bin-line align-bottom me-1""></i>حذف
            </button>");
        }

        return string.Join("", buttons);
    }
} 