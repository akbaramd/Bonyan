using Bonyan.AspNetCore.Mvc;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.Novino.Domain.Entities;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Models;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.ViewModels;
using Bonyan.UnitOfWork;
using Bonyan.User;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Bonyan.IdentityManagement.Permissions;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Permissions;
using Bonyan.Novino.Module.UserManagement.Abstractions.Zones.UserActions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Pages.User;

[Area("UserManagement")]
public class ChangePasswordModel : PageModel
{
    private readonly IBonIdentityUserRepository<Domain.Entities.User, Role> _userRepository;
    private readonly IBonIdentityUserManager<Domain.Entities.User, Role> _userManager;
    private readonly IBonCurrentUser _currentUser;
    private readonly IBonUnitOfWorkManager _unitOfWorkManager;
    private readonly IBonPermissionManager<Domain.Entities.User, Role> _permissionManager;
    private readonly ILogger<ChangePasswordModel> _logger;

    public ChangePasswordModel(
        IBonIdentityUserRepository<Domain.Entities.User, Role> userRepository,
        IBonIdentityUserManager<Domain.Entities.User, Role> userManager,
        IBonCurrentUser currentUser,
        IBonUnitOfWorkManager unitOfWorkManager,
        IBonPermissionManager<Domain.Entities.User, Role> permissionManager,
        ILogger<ChangePasswordModel> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        _unitOfWorkManager = unitOfWorkManager ?? throw new ArgumentNullException(nameof(unitOfWorkManager));
        _permissionManager = permissionManager ?? throw new ArgumentNullException(nameof(permissionManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [BindProperty]
    public ChangePasswordInputModel Input { get; set; } = new();

    public UserDetailsViewModel? UserDetails { get; set; }
    public UserChangePasswordZone ZoneModel { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        try
        {
            // Check if current user has permission to change passwords
            var currentUserId = BonUserId.NewId(_currentUser.Id!.Value);
            var canChangePassword = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.ChangePassword);
            
            if (!canChangePassword)
            {
                _logger.LogWarning("User {UserId} does not have permission to change passwords", _currentUser.Id);
                TempData["ErrorMessage"] = "شما مجوز تغییر رمز عبور را ندارید";
                return RedirectToPage("/Index");
            }

            // Validate UserId
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out _))
            {
                TempData["ErrorMessage"] = "شناسه کاربر نامعتبر است";
                return RedirectToPage("/Index");
            }

            // Get user data
            var userId = BonUserId.FromValue(Guid.Parse(id));
            var userResult = await _userManager.FindByIdAsync(userId);
            
            if (!userResult.IsSuccess)
            {
                TempData["ErrorMessage"] = "کاربر یافت نشد";
                return RedirectToPage("/Index");
            }

            var user = userResult.Value;

            // Create user details view model
            UserDetails = new UserDetailsViewModel
            {
                Id = user.Id.ToString(),
                UserName = user.UserName,
                Email = user.Email?.Address ?? "",
                Name = user.Profile.FirstName,
                SurName = user.Profile.LastName,
                IsActive = user.Status == UserStatus.Active,
                IsLocked = user.Status == UserStatus.Locked
            };

            // Initialize zone model
            ZoneModel.UserId = user.Id.ToString();

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading change password page for user {UserId}", id);
            TempData["ErrorMessage"] = "خطا در بارگذاری صفحه تغییر رمز عبور";
            return RedirectToPage("/Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            // Check if current user has permission to change passwords
            var currentUserId = BonUserId.NewId(_currentUser.Id!.Value);
            var canChangePassword = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.ChangePassword);
            
            if (!canChangePassword)
            {
                _logger.LogWarning("User {UserId} does not have permission to change passwords", _currentUser.Id);
                TempData["ErrorMessage"] = "شما مجوز تغییر رمز عبور را ندارید";
                return RedirectToPage("/Index");
            }

            if (!ModelState.IsValid)
            {
                // Reload user details for the form
                var userId = BonUserId.FromValue(Guid.Parse(Input.UserId));
                var reloadUserResult = await _userManager.FindByIdAsync(userId);
                
                if (reloadUserResult.IsSuccess)
                {
                    var reloadUser = reloadUserResult.Value;
                    UserDetails = new UserDetailsViewModel
                    {
                        Id = reloadUser.Id.ToString(),
                        UserName = reloadUser.UserName,
                        Email = reloadUser.Email?.Address ?? "",
                        Name = reloadUser.Profile.FirstName,
                        SurName = reloadUser.Profile.LastName,
                        IsActive = reloadUser.Status == UserStatus.Active,
                        IsLocked = reloadUser.Status == UserStatus.Locked
                    };
                }

                ZoneModel.UserId = Input.UserId;
                return Page();
            }

            // Validate UserId
            if (string.IsNullOrEmpty(Input.UserId) || !Guid.TryParse(Input.UserId, out _))
            {
                TempData["ErrorMessage"] = "شناسه کاربر نامعتبر است";
                return RedirectToPage("/Index");
            }

            // Get user data
            var targetUserId = BonUserId.FromValue(Guid.Parse(Input.UserId));
            var userResult = await _userManager.FindByIdAsync(targetUserId);
            
            if (!userResult.IsSuccess)
            {
                TempData["ErrorMessage"] = "کاربر یافت نشد";
                return RedirectToPage("/Index");
            }

            var user = userResult.Value;

            // Validate password confirmation
            if (Input.NewPassword != Input.ConfirmPassword)
            {
                ModelState.AddModelError("Input.ConfirmPassword", "رمز عبور و تأیید آن مطابقت ندارند");
                UserDetails = new UserDetailsViewModel
                {
                    Id = user.Id.ToString(),
                    UserName = user.UserName,
                    Email = user.Email?.Address ?? "",
                    Name = user.Profile.FirstName,
                    SurName = user.Profile.LastName,
                    IsActive = user.Status == UserStatus.Active,
                    IsLocked = user.Status == UserStatus.Locked
                };
                ZoneModel.UserId = Input.UserId;
                return Page();
            }

            // Reset password (admin action - no current password required)
            using var uow = _unitOfWorkManager.Begin();
            
            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, Input.NewPassword);
            
            if (!resetPasswordResult.IsSuccess)
            {
                ModelState.AddModelError("", "خطا در تغییر رمز عبور: " + resetPasswordResult.ErrorMessage);
                UserDetails = new UserDetailsViewModel
                {
                    Id = user.Id.ToString(),
                    UserName = user.UserName,
                    Email = user.Email?.Address ?? "",
                    Name = user.Profile.FirstName,
                    SurName = user.Profile.LastName,
                    IsActive = user.Status == UserStatus.Active,
                    IsLocked = user.Status == UserStatus.Locked
                };
                ZoneModel.UserId = Input.UserId;
                return Page();
            }

            // Set force change password if requested
            if (Input.ForceChangePassword)
            {
                user.SetForceChangePassword();
            }

            await uow.CompleteAsync();

            _logger.LogInformation("Password changed successfully for user {UserId} by {CurrentUserId}", user.Id, _currentUser.Id);
            TempData["SuccessMessage"] = $"رمز عبور کاربر {user.UserName} با موفقیت تغییر یافت";

            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user {UserId}", Input.UserId);
            TempData["ErrorMessage"] = "خطا در تغییر رمز عبور";
            return RedirectToPage("/Index");
        }
    }
}

public class ChangePasswordInputModel
{
    [Required(ErrorMessage = "شناسه کاربر الزامی است")]
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "رمز عبور جدید الزامی است")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "رمز عبور باید حداقل 6 کاراکتر باشد")]
    [DataType(DataType.Password)]
    [Display(Name = "رمز عبور جدید")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "تأیید رمز عبور الزامی است")]
    [DataType(DataType.Password)]
    [Display(Name = "تأیید رمز عبور")]
    [Compare("NewPassword", ErrorMessage = "رمز عبور و تأیید آن مطابقت ندارند")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Display(Name = "اجبار تغییر رمز عبور در ورود بعدی")]
    public bool ForceChangePassword { get; set; } = true;
} 