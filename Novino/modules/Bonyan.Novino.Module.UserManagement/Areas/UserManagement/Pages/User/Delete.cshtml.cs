using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.IdentityManagement.Permissions;
using Bonyan.Layer.Domain.DomainService;
using Bonyan.Novino.Domain.Entities;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Models;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Permissions;
using Bonyan.UnitOfWork;
using Bonyan.User;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Pages.User
{
    [Authorize(Policy = UserManagementPermissions.Users.Delete)]
    public class DeleteModel : PageModel
    {
        private readonly IBonIdentityUserManager<Domain.Entities.User, Role> _userManager;
        private readonly IBonIdentityUserRepository<Domain.Entities.User, Role> _userRepository;
        private readonly IBonPermissionManager<Domain.Entities.User, Role> _permissionManager;
        private readonly ILogger<DeleteModel> _logger;
        private readonly IBonCurrentUser _currentUser;
        private readonly IBonUnitOfWorkManager _unitOfWorkManager;

        public DeleteModel(
            IBonIdentityUserManager<Domain.Entities.User, Role> userManager,
            IBonIdentityUserRepository<Domain.Entities.User, Role> userRepository,
            IBonPermissionManager<Domain.Entities.User, Role> permissionManager,
            ILogger<DeleteModel> logger,
            IBonCurrentUser currentUser,
            IBonUnitOfWorkManager unitOfWorkManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _permissionManager = permissionManager ?? throw new ArgumentNullException(nameof(permissionManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _unitOfWorkManager = unitOfWorkManager ?? throw new ArgumentNullException(nameof(unitOfWorkManager));
        }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public string ConfirmationUsername { get; set; } = string.Empty;

        public UserDeleteViewModel User { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public bool CanDelete { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var userId = BonUserId.FromValue(Id);
                var userResult = await _userManager.FindByIdAsync(userId);
                
                if (!userResult.IsSuccess || userResult.Value == null)
                {
                    TempData["ErrorMessage"] = "کاربر مورد نظر یافت نشد.";
                    return RedirectToPage("/User/Index", new { area = "UserManagement" });
                }

                var user = userResult.Value;
                
                // Check if user can be deleted
                if (!user.CanBeDeleted)
                {
                    TempData["ErrorMessage"] = "این کاربر قابل حذف نیست.";
                    return RedirectToPage("/User/Index", new { area = "UserManagement" });
                }

                // Check if user is trying to delete themselves
                if (user.Id.Value == _currentUser.GetId())
                {
                    TempData["ErrorMessage"] = "شما نمی‌توانید حساب کاربری خود را حذف کنید.";
                    return RedirectToPage("/User/Index", new { area = "UserManagement" });
                }

                // Get user roles
                var rolesResult = await _userManager.GetUserRolesAsync(user);
                var roles = rolesResult.IsSuccess ? rolesResult.Value : new List<Role>();

                // Map to view model
                User = new UserDeleteViewModel
                {
                    Id = user.Id.ToString(),
                    UserName = user.UserName,
                    Email = user.Email?.Address ?? "",
                    Name = user.Profile.FirstName,
                    SurName = user.Profile.LastName,
                    PhoneNumber = user.PhoneNumber?.Number,
                    IsActive = user.Status == UserStatus.Active,
                    IsLocked = user.Status == UserStatus.Locked,
                    CreatedAt = user.CreatedAt,
                    EmailConfirmed = user.Email?.IsVerified ?? false,
                    PhoneNumberConfirmed = user.PhoneNumber?.IsVerified ?? false,
                    Roles = roles.Select(r => r.Title).ToList(),
                    CanBeDeleted = user.CanBeDeleted
                };

                // Check delete permission
                var currentUserId = BonUserId.FromValue(_currentUser.GetId());
                CanDelete = await _permissionManager.HasPermissionAsync(currentUserId, "UserManagement.Users.Delete");

                if (!CanDelete)
                {
                    TempData["ErrorMessage"] = "شما مجوز حذف کاربر را ندارید.";
                    return RedirectToPage("/User/Index", new { area = "UserManagement" });
                }

                ViewData["Title"] = $"حذف کاربر - {User.UserName}";
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading delete user page for ID {UserId}", Id);
                TempData["ErrorMessage"] = "خطایی در بارگذاری صفحه حذف کاربر رخ داد.";
                return RedirectToPage("/User/Index", new { area = "UserManagement" });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Load user data first
                var userId = BonUserId.FromValue(Id);
                var userResult = await _userManager.FindByIdAsync(userId);
                
                if (!userResult.IsSuccess || userResult.Value == null)
                {
                    TempData["ErrorMessage"] = "کاربر مورد نظر یافت نشد.";
                    return RedirectToPage("/User/Index", new { area = "UserManagement" });
                }

                var user = userResult.Value;
                
                // Get user roles for the User property
                var rolesResult = await _userManager.GetUserRolesAsync(user);
                var roles = rolesResult.IsSuccess ? rolesResult.Value : new List<Role>();

                // Populate User property
                User = new UserDeleteViewModel
                {
                    Id = user.Id.ToString(),
                    UserName = user.UserName,
                    Email = user.Email?.Address ?? "",
                    Name = user.Profile.FirstName,
                    SurName = user.Profile.LastName,
                    PhoneNumber = user.PhoneNumber?.Number,
                    IsActive = user.Status == UserStatus.Active,
                    IsLocked = user.Status == UserStatus.Locked,
                    CreatedAt = user.CreatedAt,
                    EmailConfirmed = user.Email?.IsVerified ?? false,
                    PhoneNumberConfirmed = user.PhoneNumber?.IsVerified ?? false,
                    Roles = roles.Select(r => r.Title).ToList(),
                    CanBeDeleted = user.CanBeDeleted
                };

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                // Validate confirmation username
                if (string.IsNullOrWhiteSpace(ConfirmationUsername))
                {
                    ModelState.AddModelError("ConfirmationUsername", "لطفاً نام کاربری را برای تأیید وارد کنید.");
                    return Page();
                }

                if (ConfirmationUsername.Trim() != User.UserName)
                {
                    ModelState.AddModelError("ConfirmationUsername", "نام کاربری وارد شده با نام کاربری کاربر مطابقت ندارد.");
                    return Page();
                }

                // Check permissions again
                var currentUserId = BonUserId.FromValue(_currentUser.GetId());
                var canDelete = await _permissionManager.HasPermissionAsync(currentUserId, "UserManagement.Users.Delete");
                
                if (!canDelete)
                {
                    TempData["ErrorMessage"] = "شما مجوز حذف کاربر را ندارید.";
                    return RedirectToPage("/User/Index", new { area = "UserManagement" });
                }

                // Final validation
                if (!user.CanBeDeleted)
                {
                    TempData["ErrorMessage"] = "این کاربر قابل حذف نیست.";
                    return RedirectToPage("/User/Index", new { area = "UserManagement" });
                }

                if (user.Id.Value == _currentUser.GetId())
                {
                    TempData["ErrorMessage"] = "شما نمی‌توانید حساب کاربری خود را حذف کنید.";
                    return RedirectToPage("/User/Index", new { area = "UserManagement" });
                }

                using var unitOfWork = _unitOfWorkManager.Begin();

                // Delete user
                var deleteResult = await _userManager.DeleteAsync(user);
                if (!deleteResult.IsSuccess)
                {
                    TempData["ErrorMessage"] = deleteResult.ErrorMessage;
                    return RedirectToPage("/User/Index", new { area = "UserManagement" });
                }

                await unitOfWork.CompleteAsync();

                _logger.LogInformation("User {UserName} deleted successfully by {CurrentUser}", 
                    User.UserName, _currentUser.GetId());

                TempData["SuccessMessage"] = $"کاربر '{User.UserName}' با موفقیت حذف شد.";
                return RedirectToPage("/User/Index", new { area = "UserManagement" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user {UserName}", User.UserName);
                TempData["ErrorMessage"] = "خطایی در حذف کاربر رخ داد.";
                return RedirectToPage("/User/Index", new { area = "UserManagement" });
            }
        }
    }
} 