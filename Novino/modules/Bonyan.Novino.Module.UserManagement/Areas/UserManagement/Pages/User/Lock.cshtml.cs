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
public class LockModel : PageModel
{
    private readonly IBonIdentityUserRepository<Domain.Entities.User, Role> _userRepository;
    private readonly IBonIdentityUserManager<Domain.Entities.User, Role> _userManager;
    private readonly IBonCurrentUser _currentUser;
    private readonly IBonUnitOfWorkManager _unitOfWorkManager;
    private readonly IBonPermissionManager<Domain.Entities.User, Role> _permissionManager;
    private readonly ILogger<LockModel> _logger;

    public LockModel(
        IBonIdentityUserRepository<Domain.Entities.User, Role> userRepository,
        IBonIdentityUserManager<Domain.Entities.User, Role> userManager,
        IBonCurrentUser currentUser,
        IBonUnitOfWorkManager unitOfWorkManager,
        IBonPermissionManager<Domain.Entities.User, Role> permissionManager,
        ILogger<LockModel> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        _unitOfWorkManager = unitOfWorkManager ?? throw new ArgumentNullException(nameof(unitOfWorkManager));
        _permissionManager = permissionManager ?? throw new ArgumentNullException(nameof(permissionManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [BindProperty]
    public LockInputModel Input { get; set; } = new();

    public UserDetailsViewModel? UserDetails { get; set; }
    public UserLockZone ZoneModel { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string id, string action)
    {
        try
        {
            // Validate action parameter
            if (string.IsNullOrEmpty(action) || (action != "lock" && action != "unlock"))
            {
                TempData["ErrorMessage"] = "عملیات نامعتبر است";
                return RedirectToPage("/Index");
            }

            // Check permissions based on action
            var currentUserId = BonUserId.NewId(_currentUser.Id!.Value);
            var canLock = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.Lock);
            var canUnlock = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.Unlock);
            
            if ((action == "lock" && !canLock) || (action == "unlock" && !canUnlock))
            {
                _logger.LogWarning("User {UserId} does not have permission to {Action} users", _currentUser.Id, action);
                TempData["ErrorMessage"] = $"شما مجوز {(action == "lock" ? "قفل کردن" : "باز کردن قفل")} کاربران را ندارید";
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

            // Validate action against current user status
            if (action == "lock" && user.Status == UserStatus.Locked)
            {
                TempData["ErrorMessage"] = "کاربر در حال حاضر قفل شده است";
                return RedirectToPage("/Index");
            }

            if (action == "unlock" && user.Status != UserStatus.Locked)
            {
                TempData["ErrorMessage"] = "کاربر قفل نشده است";
                return RedirectToPage("/Index");
            }

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
            ZoneModel.ActionType = action;

            // Initialize input model
            Input.UserId = user.Id.ToString();
            Input.ActionType = action;

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading lock/unlock page for user {UserId} with action {Action}", id, action);
            TempData["ErrorMessage"] = "خطا در بارگذاری صفحه";
            return RedirectToPage("/Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            // Check permissions based on action
            var currentUserId = BonUserId.NewId(_currentUser.Id!.Value);
            var canLock = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.Lock);
            var canUnlock = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.Unlock);
            
            if ((Input.ActionType == "lock" && !canLock) || (Input.ActionType == "unlock" && !canUnlock))
            {
                _logger.LogWarning("User {UserId} does not have permission to {Action} users", _currentUser.Id, Input.ActionType);
                TempData["ErrorMessage"] = $"شما مجوز {(Input.ActionType == "lock" ? "قفل کردن" : "باز کردن قفل")} کاربران را ندارید";
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
                ZoneModel.ActionType = Input.ActionType;
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

            // Validate action against current user status
            if (Input.ActionType == "lock" && user.Status == UserStatus.Locked)
            {
                TempData["ErrorMessage"] = "کاربر در حال حاضر قفل شده است";
                return RedirectToPage("/Index");
            }

            if (Input.ActionType == "unlock" && user.Status != UserStatus.Locked)
            {
                TempData["ErrorMessage"] = "کاربر قفل نشده است";
                return RedirectToPage("/Index");
            }

            // Perform lock/unlock action
            using var uow = _unitOfWorkManager.Begin();
            
            if (Input.ActionType == "lock")
            {
                user.Lock(Input.Reason);
                _logger.LogInformation("User {UserId} locked by {CurrentUserId} with reason: {Reason}", user.Id, _currentUser.Id, Input.Reason);
                TempData["SuccessMessage"] = $"کاربر {user.UserName} با موفقیت قفل شد";
            }
            else
            {
                user.Unlock();
                _logger.LogInformation("User {UserId} unlocked by {CurrentUserId}", user.Id, _currentUser.Id);
                TempData["SuccessMessage"] = $"قفل حساب کاربر {user.UserName} با موفقیت باز شد";
            }

            await uow.CompleteAsync();

            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing {Action} for user {UserId}", Input.ActionType, Input.UserId);
            TempData["ErrorMessage"] = $"خطا در {(Input.ActionType == "lock" ? "قفل کردن" : "باز کردن قفل")} کاربر";
            return RedirectToPage("/Index");
        }
    }
}

public class LockInputModel
{
    [Required(ErrorMessage = "شناسه کاربر الزامی است")]
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "نوع عملیات الزامی است")]
    public string ActionType { get; set; } = string.Empty;

    [Display(Name = "دلیل")]
    [StringLength(500, ErrorMessage = "دلیل نمی‌تواند بیشتر از 500 کاراکتر باشد")]
    public string? Reason { get; set; }
} 