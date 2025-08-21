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
    [Authorize(Policy = UserManagementPermissions.Users.Edit)]
    public class EditModel : PageModel
    {
        private readonly IBonIdentityUserManager<Domain.Entities.User, Role> _userManager;
        private readonly IBonIdentityUserRepository<Domain.Entities.User, Role> _userRepository;
        private readonly IBonPermissionManager<Domain.Entities.User, Role> _permissionManager;
        private readonly ILogger<EditModel> _logger;
        private readonly IBonCurrentUser _currentUser;
        private readonly IBonUnitOfWorkManager _unitOfWorkManager;

        public EditModel(
            IBonIdentityUserManager<Domain.Entities.User, Role> userManager,
            IBonIdentityUserRepository<Domain.Entities.User, Role> userRepository,
            IBonPermissionManager<Domain.Entities.User, Role> permissionManager,
            ILogger<EditModel> logger,
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
        public UserCreateEditViewModel User { get; set; } = new();

        public string? ErrorMessage { get; set; }
        public bool CanEdit { get; set; }

        private UserCreateEditViewModel MapToVm(Domain.Entities.User u) => new()
        {
            Id = u.Id.ToString(),
            UserName = u.UserName,
            Email = u.Email?.Address ?? "",
            Name = u.Profile.FirstName,
            SurName = u.Profile.LastName,
            PhoneNumber = u.PhoneNumber?.Number,
            NationalCode = u.Profile.NationalCode,
            DateOfBirth = PersianDateConverter.GregorianToPersian(u.Profile.DateOfBirth),
            IsActive = u.Status == UserStatus.Active,
            EmailConfirmed = u.Email?.IsVerified ?? false,
            PhoneNumberConfirmed = u.PhoneNumber?.IsVerified ?? false,
        };

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

                // Check edit permission
                var currentUserId = BonUserId.FromValue(_currentUser.GetId());
                CanEdit = await _permissionManager.HasPermissionAsync(currentUserId, "UserManagement.Users.Edit");

                if (!CanEdit)
                {
                    TempData["ErrorMessage"] = "شما مجوز ویرایش کاربر را ندارید.";
                    return RedirectToPage("/User/Index", new { area = "UserManagement" });
                }

                // Map to view model
                User = MapToVm(user);

                ViewData["Title"] = $"ویرایش کاربر - {User.UserName}";
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading edit user page for ID {UserId}", Id);
                TempData["ErrorMessage"] = "خطایی در بارگذاری صفحه ویرایش کاربر رخ داد.";
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

                // Check edit permission again
                var currentUserId = BonUserId.FromValue(_currentUser.GetId());
                var canEdit = await _permissionManager.HasPermissionAsync(currentUserId, "UserManagement.Users.Edit");
                
                if (!canEdit)
                {
                    TempData["ErrorMessage"] = "شما مجوز ویرایش کاربر را ندارید.";
                    return RedirectToPage("/User/Index", new { area = "UserManagement" });
                }

                if (!ModelState.IsValid)
                {
                    // Repopulate the User property for validation errors
                    User = MapToVm(user);
                    return Page();
                }

                using var unitOfWork = _unitOfWorkManager.Begin();

                // Check if username already exists (if changed)
                if (User.UserName != user.UserName)
                {
                    var existingUserResult = await _userManager.FindByUserNameAsync(User.UserName);
                    if (existingUserResult.IsSuccess && existingUserResult.Value != null)
                    {
                        ModelState.AddModelError("User.UserName", "نام کاربری قبلاً استفاده شده است.");
                        // Repopulate the User property for validation errors
                        User = MapToVm(user);
                        return Page();
                    }
                }

                // Check if email already exists (if changed)
                if (!string.IsNullOrEmpty(User.Email) && User.Email != (user.Email?.Address ?? ""))
                {
                    var existingEmailResult = await _userManager.FindByEmailAsync(User.Email);
                    if (existingEmailResult.IsSuccess && existingEmailResult.Value != null)
                    {
                        ModelState.AddModelError("User.Email", "ایمیل قبلاً استفاده شده است.");
                        // Repopulate the User property for validation errors
                        User = MapToVm(user);
                        return Page();
                    }
                }

                // Convert Persian date to Gregorian for database storage
                DateTime? gregorianDateOfBirth = !string.IsNullOrWhiteSpace(User.DateOfBirth)
                    ? PersianDateConverter.PersianToGregorian(User.DateOfBirth)
                    : null;

                if (gregorianDateOfBirth == null)
                {
                    ModelState.AddModelError("User.DateOfBirth", "تاریخ تولد معتبر نیست.");
                    User = MapToVm(user);
                    return Page();
                }

                // Update user profile
                var newProfile = new UserProfile(User.Name, User.SurName, gregorianDateOfBirth, User.NationalCode);
                user.UpdateProfile(newProfile);

                // Update email
                if (!string.IsNullOrEmpty(User.Email))
                {
                    var newEmail = new BonUserEmail(User.Email);
                    if (User.EmailConfirmed && !user.Email?.IsVerified == true)
                    {
                        newEmail.Verify();
                    }
                    else if (!User.EmailConfirmed && user.Email?.IsVerified == true)
                    {
                        newEmail.Unverify();
                    }
                    user.SetEmail(newEmail);
                }
              
                    
                // Update phone number
                if (!string.IsNullOrEmpty(User.PhoneNumber))
                {
                    var newPhone = new BonUserPhoneNumber(User.PhoneNumber);
                    if (User.PhoneNumberConfirmed && !user.PhoneNumber?.IsVerified == true)
                    {
                        newPhone.Verify();
                    }
                    else if (!User.PhoneNumberConfirmed && user.PhoneNumber?.IsVerified == true)
                    {
                        newPhone.Unverify();
                    }
                    user.SetPhoneNumber(newPhone);
                }
                

                // Update user status
                var newStatus = User.IsActive ? UserStatus.Active : UserStatus.Inactive;
                user.ChangeStatus(newStatus);

                // Update user
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.IsSuccess)
                {
                    ModelState.AddModelError("", updateResult.ErrorMessage);
                    // Repopulate the User property for validation errors
                    User = MapToVm(user);
                    return Page();
                }

                await unitOfWork.CompleteAsync();

                _logger.LogInformation("User {UserName} updated successfully by {CurrentUser}", 
                    User.UserName, _currentUser.GetId());

                TempData["SuccessMessage"] = $"کاربر '{User.UserName}' با موفقیت ویرایش شد.";
                return RedirectToPage("/User/Detail", new { id = user.Id.Value, area = "UserManagement" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user {UserName}", User.UserName);
                TempData["ErrorMessage"] = "خطایی در ویرایش کاربر رخ داد.";
                return RedirectToPage("/User/Index", new { area = "UserManagement" });
            }
        }
    }
} 