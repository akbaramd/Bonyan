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
/// Zone component that renders user detail page modals
/// </summary>
public class UserDetailPageModalZoneView : ZoneViewComponent<UserDetailPageModalZone>
{
    private readonly IBonIdentityUserRepository<Domain.Entities.User, Role> _userRepository;
    private readonly IBonIdentityUserManager<Domain.Entities.User, Role> _userManager;
    private readonly IBonCurrentUser _currentUser;
    private readonly IBonUnitOfWorkManager _unitOfWorkManager;
    private readonly IBonPermissionManager<Domain.Entities.User, Role> _permissionManager;
    private readonly ILogger<UserDetailPageModalZoneView> _logger;

    public UserDetailPageModalZoneView(
        IBonIdentityUserRepository<Domain.Entities.User, Role> userRepository,
        IBonIdentityUserManager<Domain.Entities.User, Role> userManager,
        IBonCurrentUser currentUser,
        IBonUnitOfWorkManager unitOfWorkManager,
        IBonPermissionManager<Domain.Entities.User, Role> permissionManager,
        ILogger<UserDetailPageModalZoneView> logger)
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
    public override string Id => "user_detail_page_modal";

    public override int Priority => 100;

    public override async Task<ZoneComponentResult> HandleAsync(UserDetailPageModalZone model, ZoneRenderingContext context, ZoneComponentParameters parameters, CancellationToken cancellationToken = default)
    {
        var logger = context.GetServiceOrDefault<IZoneComponentLogger>();
        logger?.LogInformation(Id, $"Rendering modals for user: {model.UserId ?? "Unknown"}");

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Check if current user has permission to view user information
            var currentUserId = BonUserId.NewId(_currentUser.Id!.Value);
            var canViewUser = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.View);
            
            if (!canViewUser)
            {
                _logger.LogInformation("User {UserId} does not have permission to view user information", _currentUser.Id);
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
            var canChangePassword = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.ChangePassword);
            var canDeleteUser = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.Delete);
            var canLockUser = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.Lock);
            var canUnlockUser = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.Unlock);
            var canActivateUser = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.Activate);
            var canDeactivateUser = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.Deactivate);

            // Create user details for modal context
            var userDetails = new UserDetailsViewModel
            {
                Id = user.Id.ToString(),
                UserName = user.UserName,
                Name = user.Profile.FirstName,
                SurName = user.Profile.LastName,
                IsActive = user.Status == UserStatus.Active,
                IsLocked = user.Status == UserStatus.Locked,
                CanBeDeleted = user.CanBeDeleted
            };

            // Render the modals HTML
            var modalsHtml = $@"
                {GetChangePasswordModal(userDetails, canChangePassword)}
                {GetDeleteConfirmationModal(userDetails, canDeleteUser)}
                {GetLockUserModal(userDetails, canLockUser)}
                {GetUnlockUserModal(userDetails, canUnlockUser)}
                {GetActivateUserModal(userDetails, canActivateUser)}
                {GetDeactivateUserModal(userDetails, canDeactivateUser)}";

            logger?.LogInformation(Id, $"Successfully rendered modals for user: {userDetails.UserName}");

            // Add metadata to zone context for other components
            context.AddMeta("userDetails", userDetails);
            context.AddMeta("canChangePassword", canChangePassword);
            context.AddMeta("canDelete", canDeleteUser);
            context.AddMeta("canLock", canLockUser);
            context.AddMeta("canUnlock", canUnlockUser);
            context.AddMeta("canActivate", canActivateUser);
            context.AddMeta("canDeactivate", canDeactivateUser);

            return ZoneComponentResult.Html(modalsHtml, true);
        }
        catch (OperationCanceledException)
        {
            logger?.LogWarning(Id, "Modals rendering was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            logger?.LogError(Id, "Error rendering user detail modals", ex);
            throw;
        }
    }

    private string GetChangePasswordModal(UserDetailsViewModel user, bool canChangePassword)
    {
        if (!canChangePassword)
            return string.Empty;

        return $@"
            <div class=""modal fade"" id=""changePasswordModal"" tabindex=""-1"" aria-labelledby=""changePasswordModalLabel"" aria-hidden=""true"">
                <div class=""modal-dialog modal-dialog-centered"">
                    <div class=""modal-content"">
                        <div class=""modal-header"">
                            <h5 class=""modal-title"" id=""changePasswordModalLabel"">
                                <i class=""ri-lock-password-line me-2""></i>تغییر رمز عبور
                            </h5>
                            <button type=""button"" class=""btn-close"" data-bs-dismiss=""modal"" aria-label=""Close""></button>
                        </div>
                        <form method=""post"" asp-page-handler=""ChangePassword"" id=""changePasswordForm"">
                            <div class=""modal-body"">
                                <input type=""hidden"" name=""UserId"" value=""{user.Id}"" />
                                
                                <div class=""mb-3"">
                                    <label class=""form-label"">نوع تغییر رمز</label>
                                    <div class=""d-flex gap-3"">
                                        <div class=""form-check"">
                                            <input class=""form-check-input"" type=""radio"" name=""PasswordChangeType"" id=""manualPassword"" value=""manual"" checked>
                                            <label class=""form-check-label"" for=""manualPassword"">
                                                تعیین دستی
                                            </label>
                                        </div>
                                        <div class=""form-check"">
                                            <input class=""form-check-input"" type=""radio"" name=""PasswordChangeType"" id=""randomPassword"" value=""random"">
                                            <label class=""form-check-label"" for=""randomPassword"">
                                                تولید خودکار
                                            </label>
                                        </div>
                                    </div>
                                </div>
                                
                                <div id=""manualPasswordSection"">
                                    <div class=""mb-3"">
                                        <label class=""form-label"">رمز عبور جدید</label>
                                        <div class=""input-group"">
                                            <input type=""password"" class=""form-control"" name=""NewPassword"" id=""newPassword"" />
                                            <button class=""btn btn-outline-secondary"" type=""button"" id=""toggleNewPassword"">
                                                <i class=""ri-eye-line""></i>
                                            </button>
                                        </div>
                                    </div>
                                    <div class=""mb-3"">
                                        <label class=""form-label"">تأیید رمز عبور</label>
                                        <div class=""input-group"">
                                            <input type=""password"" class=""form-control"" name=""ConfirmPassword"" id=""confirmPassword"" />
                                            <button class=""btn btn-outline-secondary"" type=""button"" id=""toggleConfirmPassword"">
                                                <i class=""ri-eye-line""></i>
                                            </button>
                                        </div>
                                    </div>
                                </div>
                                
                                <div id=""randomPasswordSection"" style=""display: none;"">
                                    <div class=""alert alert-info"">
                                        <i class=""ri-information-line me-2""></i>
                                        رمز عبور جدید به صورت خودکار تولید خواهد شد و برای کاربر ارسال می‌شود.
                                    </div>
                                </div>
                                
                                <div class=""mb-3"">
                                    <div class=""form-check"">
                                        <input class=""form-check-input"" type=""checkbox"" name=""ForceChangePassword"" id=""forceChangePassword"" checked>
                                        <label class=""form-check-label"" for=""forceChangePassword"">
                                            اجبار تغییر رمز عبور در ورود بعدی
                                        </label>
                                    </div>
                                </div>
                            </div>
                            <div class=""modal-footer"">
                                <button type=""button"" class=""btn btn-light"" data-bs-dismiss=""modal"">انصراف</button>
                                <button type=""submit"" class=""btn btn-primary"">
                                    <i class=""ri-save-line align-bottom me-1""></i>تغییر رمز عبور
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>";
    }

    private string GetDeleteConfirmationModal(UserDetailsViewModel user, bool canDelete)
    {
        if (!canDelete || !user.CanBeDeleted)
            return string.Empty;

        return $@"
            <div class=""modal fade"" id=""deleteUserModal"" tabindex=""-1"" aria-labelledby=""deleteUserModalLabel"" aria-hidden=""true"">
                <div class=""modal-dialog modal-dialog-centered"">
                    <div class=""modal-content"">
                        <div class=""modal-header"">
                            <h5 class=""modal-title"" id=""deleteUserModalLabel"">
                                <i class=""ri-delete-bin-line me-2""></i>حذف کاربر
                            </h5>
                            <button type=""button"" class=""btn-close"" data-bs-dismiss=""modal"" aria-label=""Close""></button>
                        </div>
                        <div class=""modal-body"">
                            <div class=""alert alert-danger"">
                                <i class=""ri-error-warning-line me-2""></i>
                                <strong>هشدار:</strong> آیا از حذف کاربر ""{user.FullName}"" اطمینان دارید؟
                            </div>
                            <p class=""text-muted"">این عملیات قابل بازگشت نیست و تمام اطلاعات کاربر حذف خواهد شد.</p>
                        </div>
                        <div class=""modal-footer"">
                            <button type=""button"" class=""btn btn-light"" data-bs-dismiss=""modal"">انصراف</button>
                            <button type=""button"" class=""btn btn-danger"" onclick=""confirmDeleteUser('{user.Id}', '{user.UserName}')"">
                                <i class=""ri-delete-bin-line align-bottom me-1""></i>حذف
                            </button>
                        </div>
                    </div>
                </div>
            </div>";
    }

    private string GetLockUserModal(UserDetailsViewModel user, bool canLock)
    {
        if (!canLock || user.IsLocked)
            return string.Empty;

        return $@"
            <div class=""modal fade"" id=""lockUserModal"" tabindex=""-1"" aria-labelledby=""lockUserModalLabel"" aria-hidden=""true"">
                <div class=""modal-dialog modal-dialog-centered"">
                    <div class=""modal-content"">
                        <div class=""modal-header"">
                            <h5 class=""modal-title"" id=""lockUserModalLabel"">
                                <i class=""ri-lock-line me-2""></i>قفل کردن کاربر
                            </h5>
                            <button type=""button"" class=""btn-close"" data-bs-dismiss=""modal"" aria-label=""Close""></button>
                        </div>
                        <div class=""modal-body"">
                            <p>آیا از قفل کردن کاربر ""{user.FullName}"" اطمینان دارید؟</p>
                            <p class=""text-muted"">کاربر قفل شده نمی‌تواند وارد سیستم شود.</p>
                        </div>
                        <div class=""modal-footer"">
                            <button type=""button"" class=""btn btn-light"" data-bs-dismiss=""modal"">انصراف</button>
                            <button type=""button"" class=""btn btn-warning"" onclick=""confirmLockUser('{user.Id}', '{user.UserName}')"">
                                <i class=""ri-lock-line align-bottom me-1""></i>قفل کردن
                            </button>
                        </div>
                    </div>
                </div>
            </div>";
    }

    private string GetUnlockUserModal(UserDetailsViewModel user, bool canUnlock)
    {
        if (!canUnlock || !user.IsLocked)
            return string.Empty;

        return $@"
            <div class=""modal fade"" id=""unlockUserModal"" tabindex=""-1"" aria-labelledby=""unlockUserModalLabel"" aria-hidden=""true"">
                <div class=""modal-dialog modal-dialog-centered"">
                    <div class=""modal-content"">
                        <div class=""modal-header"">
                            <h5 class=""modal-title"" id=""unlockUserModalLabel"">
                                <i class=""ri-unlock-line me-2""></i>باز کردن قفل کاربر
                            </h5>
                            <button type=""button"" class=""btn-close"" data-bs-dismiss=""modal"" aria-label=""Close""></button>
                        </div>
                        <div class=""modal-body"">
                            <p>آیا از باز کردن قفل کاربر ""{user.FullName}"" اطمینان دارید؟</p>
                            <p class=""text-muted"">کاربر می‌تواند دوباره وارد سیستم شود.</p>
                        </div>
                        <div class=""modal-footer"">
                            <button type=""button"" class=""btn btn-light"" data-bs-dismiss=""modal"">انصراف</button>
                            <button type=""button"" class=""btn btn-success"" onclick=""confirmUnlockUser('{user.Id}', '{user.UserName}')"">
                                <i class=""ri-unlock-line align-bottom me-1""></i>باز کردن قفل
                            </button>
                        </div>
                    </div>
                </div>
            </div>";
    }

    private string GetActivateUserModal(UserDetailsViewModel user, bool canActivate)
    {
        if (!canActivate || user.IsActive)
            return string.Empty;

        return $@"
            <div class=""modal fade"" id=""activateUserModal"" tabindex=""-1"" aria-labelledby=""activateUserModalLabel"" aria-hidden=""true"">
                <div class=""modal-dialog modal-dialog-centered"">
                    <div class=""modal-content"">
                        <div class=""modal-header"">
                            <h5 class=""modal-title"" id=""activateUserModalLabel"">
                                <i class=""ri-check-line me-2""></i>فعال کردن کاربر
                            </h5>
                            <button type=""button"" class=""btn-close"" data-bs-dismiss=""modal"" aria-label=""Close""></button>
                        </div>
                        <div class=""modal-body"">
                            <p>آیا از فعال کردن کاربر ""{user.FullName}"" اطمینان دارید؟</p>
                            <p class=""text-muted"">کاربر فعال می‌تواند وارد سیستم شود.</p>
                        </div>
                        <div class=""modal-footer"">
                            <button type=""button"" class=""btn btn-light"" data-bs-dismiss=""modal"">انصراف</button>
                            <button type=""button"" class=""btn btn-success"" onclick=""confirmActivateUser('{user.Id}', '{user.UserName}')"">
                                <i class=""ri-check-line align-bottom me-1""></i>فعال کردن
                            </button>
                        </div>
                    </div>
                </div>
            </div>";
    }

    private string GetDeactivateUserModal(UserDetailsViewModel user, bool canDeactivate)
    {
        if (!canDeactivate || !user.IsActive)
            return string.Empty;

        return $@"
            <div class=""modal fade"" id=""deactivateUserModal"" tabindex=""-1"" aria-labelledby=""deactivateUserModalLabel"" aria-hidden=""true"">
                <div class=""modal-dialog modal-dialog-centered"">
                    <div class=""modal-content"">
                        <div class=""modal-header"">
                            <h5 class=""modal-title"" id=""deactivateUserModalLabel"">
                                <i class=""ri-close-line me-2""></i>غیرفعال کردن کاربر
                            </h5>
                            <button type=""button"" class=""btn-close"" data-bs-dismiss=""modal"" aria-label=""Close""></button>
                        </div>
                        <div class=""modal-body"">
                            <p>آیا از غیرفعال کردن کاربر ""{user.FullName}"" اطمینان دارید؟</p>
                            <p class=""text-muted"">کاربر غیرفعال نمی‌تواند وارد سیستم شود.</p>
                        </div>
                        <div class=""modal-footer"">
                            <button type=""button"" class=""btn btn-light"" data-bs-dismiss=""modal"">انصراف</button>
                            <button type=""button"" class=""btn btn-danger"" onclick=""confirmDeactivateUser('{user.Id}', '{user.UserName}')"">
                                <i class=""ri-close-line align-bottom me-1""></i>غیرفعال کردن
                            </button>
                        </div>
                    </div>
                </div>
            </div>";
    }
} 