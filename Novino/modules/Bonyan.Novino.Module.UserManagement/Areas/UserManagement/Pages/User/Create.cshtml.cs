using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.IdentityManagement.Permissions;
using Bonyan.Layer.Domain.DomainService;
using Bonyan.Novino.Domain.Entities;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Models;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Permissions;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Utils;
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
    [Authorize(Policy = UserManagementPermissions.Users.Create)]
    public class CreateModel : PageModel
    {
        private readonly IBonIdentityUserManager<Domain.Entities.User, Role> _userManager;
        private readonly IBonIdentityUserRepository<Domain.Entities.User, Role> _userRepository;
        private readonly IBonPermissionManager<Domain.Entities.User, Role> _permissionManager;
        private readonly ILogger<CreateModel> _logger;
        private readonly IBonCurrentUser _currentUser;
        private readonly IBonUnitOfWorkManager _unitOfWorkManager;

        public CreateModel(
            IBonIdentityUserManager<Domain.Entities.User, Role> userManager,
            IBonIdentityUserRepository<Domain.Entities.User, Role> userRepository,
            IBonPermissionManager<Domain.Entities.User, Role> permissionManager,
            ILogger<CreateModel> logger,
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

        [BindProperty]
        public UserCreateEditViewModel User { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Set default password for new users
                User.Password = "123456"; // Default password
                User.ConfirmPassword = "123456";
                User.IsActive = true;
                User.EmailConfirmed = false;
                User.ForceChangePassword = true; // Force password change on first login
                
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading create user page");
                ErrorMessage = "خطایی در بارگذاری صفحه ایجاد کاربر رخ داد.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                using var unitOfWork = _unitOfWorkManager.Begin();

                // Check if username already exists
                var existingUserResult = await _userManager.FindByUserNameAsync(User.UserName);
                if (existingUserResult.IsSuccess && existingUserResult.Value != null)
                {
                    ModelState.AddModelError("User.UserName", "نام کاربری قبلاً استفاده شده است.");
                    return Page();
                }

                // Check if email already exists
                if (!string.IsNullOrEmpty(User.Email))
                {
                    var existingEmailResult = await _userManager.FindByEmailAsync(User.Email);
                    if (existingEmailResult.IsSuccess && existingEmailResult.Value != null)
                    {
                        ModelState.AddModelError("User.Email", "ایمیل قبلاً استفاده شده است.");
                        return Page();
                    }
                }

                // Convert Persian date to Gregorian for database storage
                DateTime? gregorianDateOfBirth = null;
                if (!string.IsNullOrWhiteSpace(User.DateOfBirth))
                {
                    gregorianDateOfBirth = PersianDateConverter.PersianToGregorian(User.DateOfBirth);
                    if (!gregorianDateOfBirth.HasValue)
                    {
                        ModelState.AddModelError("User.DateOfBirth", "تاریخ تولد وارد شده معتبر نیست.");
                        return Page();
                    }
                }

                // Create new user with default password
                var userId = BonUserId.NewId();
                var userProfile = new UserProfile(User.Name, User.SurName, gregorianDateOfBirth, User.NationalCode);
                
                var user = new Domain.Entities.User(
                    userId,
                    User.UserName,
                    userProfile
                );

                // Set email and phone if provided
                if (!string.IsNullOrEmpty(User.Email))
                {
                    user.SetEmail(new BonUserEmail(User.Email));
                    if (User.EmailConfirmed)
                    {
                        user.Email.Verify();
                    }
                }

                if (!string.IsNullOrEmpty(User.PhoneNumber))
                {
                    user.SetPhoneNumber(new BonUserPhoneNumber(User.PhoneNumber));
                }

                // Set user status
                user.ChangeStatus(User.IsActive ? UserStatus.Active : UserStatus.Inactive);

                // Create user with password
                var createResult = await _userManager.CreateAsync(user, User.Password);
                if (!createResult.IsSuccess)
                {
                    ModelState.AddModelError("", createResult.ErrorMessage);
                    return Page();
                }

                // Force password change if requested
                if (User.ForceChangePassword)
                {
                    // Add a claim to force password change on next login
                    await _userManager.AddClaimAsync(user, "ForcePasswordChange", "true");
                }

                await unitOfWork.CompleteAsync();

                _logger.LogInformation("User {UserName} created successfully by {CurrentUser}", 
                    User.UserName, _currentUser.GetId());

                // Redirect to user list with success message
                TempData["SuccessMessage"] = $"کاربر '{User.UserName}' با موفقیت ایجاد شد. رمز عبور پیش‌فرض: 123456";
                return RedirectToPage("/User/Detail", new {id=user.Id.Value, area = "UserManagement" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user {UserName}", User.UserName);
                ErrorMessage = "خطایی در ایجاد کاربر رخ داد.";
                return Page();
            }
        }
    }
} 