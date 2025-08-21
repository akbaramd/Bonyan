using Bonyan.AspNetCore.ZoneComponent;
using Bonyan.Novino.Module.UserManagement.Abstractions.Zones.UserActions;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.ViewModels;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.Novino.Domain.Entities;
using Bonyan.User;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Bonyan.IdentityManagement.Permissions;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Permissions;
using Microsoft.Extensions.Logging;

namespace Bonyan.Novino.Module.UserManagement.Areas.UserManagement.ZoneViews;

/// <summary>
/// Zone component that renders additional fields for change password form
/// This demonstrates how other modules can extend the change password functionality
/// </summary>
public class UserChangePasswordFormZoneView : ZoneViewComponent<UserChangePasswordZone>
{
    private readonly IBonIdentityUserRepository<Domain.Entities.User, Role> _userRepository;
    private readonly IBonIdentityUserManager<Domain.Entities.User, Role> _userManager;
    private readonly IBonCurrentUser _currentUser;
    private readonly IBonPermissionManager<Domain.Entities.User, Role> _permissionManager;
    private readonly ILogger<UserChangePasswordFormZoneView> _logger;

    public UserChangePasswordFormZoneView(
        IBonIdentityUserRepository<Domain.Entities.User, Role> userRepository,
        IBonIdentityUserManager<Domain.Entities.User, Role> userManager,
        IBonCurrentUser currentUser,
        IBonPermissionManager<Domain.Entities.User, Role> permissionManager,
        ILogger<UserChangePasswordFormZoneView> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        _permissionManager = permissionManager ?? throw new ArgumentNullException(nameof(permissionManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Custom unique ID to prevent duplicates
    /// </summary>
    public override string Id => "user_change_password_form";

    public override int Priority => 100;

    public override async Task<ZoneComponentResult> HandleAsync(UserChangePasswordZone model, ZoneRenderingContext context, ZoneComponentParameters parameters, CancellationToken cancellationToken = default)
    {
        var logger = context.GetServiceOrDefault<IZoneComponentLogger>();
        logger?.LogInformation(Id, $"Rendering change password form for user: {model.UserId ?? "Unknown"}");

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Check if current user has permission to change passwords
            var currentUserId = BonUserId.NewId(_currentUser.Id!.Value);
            var canChangePassword = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.ChangePassword);

            if (!canChangePassword)
            {
                _logger.LogInformation("User {UserId} does not have permission to change passwords", _currentUser.Id);
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

            // Create additional form fields HTML
            var additionalFieldsHtml = GenerateAdditionalFields(user);

            // Add custom validation script
            var validationScript = GenerateValidationScript();

            // Add metadata to zone context for other components
            context.AddMeta("userDetails", new { Id = user.Id.ToString(), UserName = user.UserName, Email = user.Email?.Address });
            context.AddMeta("canChangePassword", canChangePassword);

            logger?.LogInformation(Id, $"Successfully rendered change password form for user: {user.UserName}");

            return ZoneComponentResult.Html(additionalFieldsHtml, true);
        }
        catch (OperationCanceledException)
        {
            logger?.LogWarning(Id, "Change password form rendering was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            logger?.LogError(Id, "Error rendering change password form", ex);
            throw;
        }
    }

    /// <summary>
    /// Generate additional form fields that can be added by other modules
    /// </summary>
    private string GenerateAdditionalFields(Domain.Entities.User user)
    {
        var html = new System.Text.StringBuilder();

        // Example: Add a notification checkbox
        html.AppendLine(@"
            <div class=""mb-3"">
                <div class=""form-check"">
                    <input type=""checkbox"" class=""form-check-input"" id=""sendNotification"" name=""sendNotification"" value=""true"">
                    <label class=""form-check-label"" for=""sendNotification"">
                        <i class=""ri-notification-line me-1""></i>ارسال اعلان به کاربر
                    </label>
                </div>
                <small class=""text-muted"">
                    در صورت انتخاب، اعلان تغییر رمز عبور به کاربر ارسال خواهد شد.
                </small>
            </div>");

        // Example: Add a reason field
        html.AppendLine(@"
            <div class=""mb-3"">
                <label for=""changeReason"" class=""form-label"">
                    <i class=""ri-message-2-line me-1""></i>دلیل تغییر رمز عبور
                </label>
                <select class=""form-select"" id=""changeReason"" name=""changeReason"">
                    <option value="""">انتخاب کنید</option>
                    <option value=""forgot"">فراموشی رمز عبور</option>
                    <option value=""security"">درخواست امنیتی</option>
                    <option value=""admin"">درخواست مدیر</option>
                    <option value=""other"">سایر</option>
                </select>
                <small class=""text-muted"">
                    دلیل تغییر رمز عبور را انتخاب کنید (اختیاری).
                </small>
            </div>");

        // Example: Add a custom note field
        html.AppendLine(@"
            <div class=""mb-3"">
                <label for=""adminNote"" class=""form-label"">
                    <i class=""ri-file-text-line me-1""></i>یادداشت مدیر
                </label>
                <textarea class=""form-control"" id=""adminNote"" name=""adminNote"" rows=""2"" 
                          placeholder=""یادداشت اختیاری برای این عملیات...""></textarea>
                <small class=""text-muted"">
                    یادداشت داخلی که فقط برای مدیران قابل مشاهده است.
                </small>
            </div>");

        return html.ToString();
    }

    /// <summary>
    /// Generate custom validation script
    /// </summary>
    private string GenerateValidationScript()
    {
        return @"
            <script>
                // Custom validation for change password form
                document.addEventListener('DOMContentLoaded', function() {
                    const changeReason = document.getElementById('changeReason');
                    const adminNote = document.getElementById('adminNote');
                    
                    // Add validation for reason field
                    if (changeReason) {
                        changeReason.addEventListener('change', function() {
                            if (this.value === 'other' && adminNote) {
                                adminNote.setAttribute('required', 'required');
                                adminNote.placeholder = 'در صورت انتخاب سایر، لطفاً دلیل را توضیح دهید...';
                            } else if (adminNote) {
                                adminNote.removeAttribute('required');
                                adminNote.placeholder = 'یادداشت اختیاری برای این عملیات...';
                            }
                        });
                    }
                });
            </script>";
    }
} 