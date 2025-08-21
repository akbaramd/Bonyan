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
public class DeleteConfirmModel : PageModel
{
    private readonly IBonIdentityUserRepository<Domain.Entities.User, Role> _userRepository;
    private readonly IBonIdentityUserManager<Domain.Entities.User, Role> _userManager;
    private readonly IBonCurrentUser _currentUser;
    private readonly IBonUnitOfWorkManager _unitOfWorkManager;
    private readonly IBonPermissionManager<Domain.Entities.User, Role> _permissionManager;
    private readonly ILogger<DeleteConfirmModel> _logger;

    public DeleteConfirmModel(
        IBonIdentityUserRepository<Domain.Entities.User, Role> userRepository,
        IBonIdentityUserManager<Domain.Entities.User, Role> userManager,
        IBonCurrentUser currentUser,
        IBonUnitOfWorkManager unitOfWorkManager,
        IBonPermissionManager<Domain.Entities.User, Role> permissionManager,
        ILogger<DeleteConfirmModel> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        _unitOfWorkManager = unitOfWorkManager ?? throw new ArgumentNullException(nameof(unitOfWorkManager));
        _permissionManager = permissionManager ?? throw new ArgumentNullException(nameof(permissionManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [BindProperty]
    public DeleteConfirmInputModel Input { get; set; } = new();

    public UserDetailsViewModel? UserDetails { get; set; }
    public UserDeleteZone ZoneModel { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        try
        {
            // Check if current user has permission to delete users
            var currentUserId = BonUserId.NewId(_currentUser.Id!.Value);
            var canDeleteUser = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.Delete);
            
            if (!canDeleteUser)
            {
                _logger.LogWarning("User {UserId} does not have permission to delete users", _currentUser.Id);
                TempData["ErrorMessage"] = "شما مجوز حذف کاربران را ندارید";
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

            // Check if user can be deleted
            if (!user.CanBeDeleted)
            {
                TempData["ErrorMessage"] = "این کاربر قابل حذف نیست";
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

            // Initialize input model
            Input.UserId = user.Id.ToString();
            Input.ExpectedUsername = user.UserName;

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading delete confirmation page for user {UserId}", id);
            TempData["ErrorMessage"] = "خطا در بارگذاری صفحه تأیید حذف";
            return RedirectToPage("/Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            // Check if current user has permission to delete users
            var currentUserId = BonUserId.NewId(_currentUser.Id!.Value);
            var canDeleteUser = await _permissionManager.HasPermissionAsync(currentUserId, UserManagementPermissions.Users.Delete);
            
            if (!canDeleteUser)
            {
                _logger.LogWarning("User {UserId} does not have permission to delete users", _currentUser.Id);
                TempData["ErrorMessage"] = "شما مجوز حذف کاربران را ندارید";
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

            // Check if user can be deleted
            if (!user.CanBeDeleted)
            {
                TempData["ErrorMessage"] = "این کاربر قابل حذف نیست";
                return RedirectToPage("/Index");
            }

            // Validate username confirmation
            if (Input.ConfirmedUsername != Input.ExpectedUsername)
            {
                ModelState.AddModelError("Input.ConfirmedUsername", "نام کاربری وارد شده صحیح نیست");
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

            // Delete user
            using var uow = _unitOfWorkManager.Begin();
            
            var deleteResult = await _userManager.DeleteAsync(user);
            
            if (!deleteResult.IsSuccess)
            {
                ModelState.AddModelError("", "خطا در حذف کاربر: " + deleteResult.Error);
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

            await uow.CompleteAsync();

            _logger.LogInformation("User {UserId} deleted successfully by {CurrentUserId}", user.Id, _currentUser.Id);
            TempData["SuccessMessage"] = $"کاربر {user.UserName} با موفقیت حذف شد";

            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", Input.UserId);
            TempData["ErrorMessage"] = "خطا در حذف کاربر";
            return RedirectToPage("/Index");
        }
    }
}

public class DeleteConfirmInputModel
{
    [Required(ErrorMessage = "شناسه کاربر الزامی است")]
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "نام کاربری الزامی است")]
    [Display(Name = "نام کاربری")]
    public string ConfirmedUsername { get; set; } = string.Empty;

    public string ExpectedUsername { get; set; } = string.Empty;
} 