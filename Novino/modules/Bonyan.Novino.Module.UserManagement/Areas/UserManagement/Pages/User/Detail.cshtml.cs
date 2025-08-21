using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.IdentityManagement.Permissions;
using Bonyan.Layer.Domain.DomainService;
using Bonyan.Novino.Domain.Entities;
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
using System.Security.Cryptography;
using System.Text;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Models;

namespace Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Pages.User
{
    [Authorize(Policy = UserManagementPermissions.Users.View)]
    public class DetailModel : PageModel
    {
        private readonly IBonIdentityUserManager<Domain.Entities.User, Role> _userManager;
        private readonly IBonIdentityUserRepository<Domain.Entities.User, Role> _userRepository;
        private readonly IBonPermissionManager<Domain.Entities.User, Role> _permissionManager;
        private readonly ILogger<DetailModel> _logger;
        private readonly IBonCurrentUser _currentUser;
        private readonly IBonUnitOfWorkManager _unitOfWorkManager;

        public DetailModel(
            IBonIdentityUserManager<Domain.Entities.User, Role> userManager,
            IBonIdentityUserRepository<Domain.Entities.User, Role> userRepository,
            IBonPermissionManager<Domain.Entities.User, Role> permissionManager,
            ILogger<DetailModel> logger,
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

        public UserDetailsViewModel User { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        // Permission flags
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanChangePassword { get; set; }
        public bool CanViewClaims { get; set; }
        public bool CanViewActivity { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                var userId = BonUserId.FromValue(id);
                var userResult = await _userManager.FindByIdAsync(userId);
                
                if (!userResult.IsSuccess || userResult.Value == null)
                {
                    ErrorMessage = "کاربر مورد نظر یافت نشد.";
                    return RedirectToPage("/User/Index", new { area = "UserManagement" });
                }

                var user = userResult.Value;
                
                // Get user roles
                var rolesResult = await _userManager.GetUserRolesAsync(user);
                var roles = rolesResult.IsSuccess ? rolesResult.Value : new List<Role>();

                // Get user claims
                var claimsResult = await _userManager.GetAllClaimsAsync(user);
                var claims = claimsResult.IsSuccess ? claimsResult.Value : new List<BonIdentityUserClaims<Domain.Entities.User, Role>>();

                // Map to view model
                User = new UserDetailsViewModel
                {
                    Id = user.Id.ToString(),
                    UserName = user.UserName,
                    Email = user.Email?.Address ?? "",
                    Name = user.Profile.FirstName,
                    SurName = user.Profile.LastName,
                    PhoneNumber = user.PhoneNumber?.Number,
                    NationalCode = user.Profile.NationalCode,
                    DateOfBirth = PersianDateConverter.GregorianToPersian(user.Profile.DateOfBirth),    
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
                    Claims = claims.Select(c => new UserClaimViewModel
                    {
                        Id = c.Id.ToString(),
                        ClaimType = c.ClaimType,
                        ClaimValue = c.ClaimValue,
                        Issuer = c.Issuer,
                    }).ToList()
                };

                // Check permissions
                var currentUserId = BonUserId.FromValue(_currentUser.GetId());
                CanEdit = await _permissionManager.HasPermissionAsync(currentUserId, "UserManagement.Users.Update");
                CanDelete = await _permissionManager.HasPermissionAsync(currentUserId, "UserManagement.Users.Delete");
                CanChangePassword = await _permissionManager.HasPermissionAsync(currentUserId, "UserManagement.Users.ChangePassword");
                CanViewClaims = await _permissionManager.HasPermissionAsync(currentUserId, "UserManagement.Users.ManageClaims");
                CanViewActivity = await _permissionManager.HasPermissionAsync(currentUserId, "UserManagement.Users.ViewActivity");

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading user details for ID {UserId}", id);
                ErrorMessage = "خطایی در بارگذاری جزئیات کاربر رخ داد.";
                return RedirectToPage("/User/Index", new { area = "UserManagement" });
            }
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            try
            {
                var userId = Request.Form["UserId"].ToString();
                var passwordChangeType = Request.Form["PasswordChangeType"].ToString();
                var forceChangePassword = Request.Form.ContainsKey("ForceChangePassword");

                Guid userGuid;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out userGuid))
                {
                    ErrorMessage = "شناسه کاربر نامعتبر است.";
                    return RedirectToPage("/User/Index", new { area = "UserManagement" });
                }

                var bonUserId = BonUserId.FromValue(userGuid);
                var userResult = await _userManager.FindByIdAsync(bonUserId);
                
                if (!userResult.IsSuccess || userResult.Value == null)
                {
                    ErrorMessage = "کاربر مورد نظر یافت نشد.";
                    return RedirectToPage("/User/Index", new { area = "UserManagement" });
                }

                var user = userResult.Value;
                string newPassword;

                if (passwordChangeType == "random")
                {
                    // Generate random password
                    newPassword = GenerateRandomPassword();
                }
                else
                {
                    // Manual password
                    newPassword = Request.Form["NewPassword"].ToString();
                    var confirmPassword = Request.Form["ConfirmPassword"].ToString();

                    if (string.IsNullOrEmpty(newPassword))
                    {
                        ErrorMessage = "رمز عبور جدید الزامی است.";
                        return RedirectToPage(new { id = userGuid });
                    }

                    if (newPassword != confirmPassword)
                    {
                        ErrorMessage = "رمز عبور و تأیید آن مطابقت ندارند.";
                        return RedirectToPage(new { id = userGuid });
                    }

                    if (newPassword.Length < 6)
                    {
                        ErrorMessage = "رمز عبور باید حداقل 6 کاراکتر باشد.";
                        return RedirectToPage(new { id = userGuid });
                    }
                }

                using var unitOfWork = _unitOfWorkManager.Begin();

                // Reset password
                var resetResult = await _userManager.ResetPasswordAsync(user, newPassword);
                if (!resetResult.IsSuccess)
                {
                    ErrorMessage = resetResult.ErrorMessage;
                    return RedirectToPage(new { id = userGuid });
                }

                // Force password change if requested
                if (forceChangePassword)
                {
                    await _userManager.AddClaimAsync(user, "ForcePasswordChange", "true");
                }

                await unitOfWork.CompleteAsync();

                _logger.LogInformation("Password changed for user {UserName} by {CurrentUser}", 
                    user.UserName, _currentUser.GetId());

                if (passwordChangeType == "random")
                {
                    SuccessMessage = $"رمز عبور کاربر با موفقیت تغییر یافت. رمز عبور جدید: {newPassword}";
                }
                else
                {
                    SuccessMessage = "رمز عبور کاربر با موفقیت تغییر یافت.";
                }

                return RedirectToPage(new { id = userGuid });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing password");
                ErrorMessage = "خطایی در تغییر رمز عبور رخ داد.";
                return RedirectToPage();
            }
        }

        private string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
            var random = new Random();
            var password = new StringBuilder();

            // Ensure at least one character from each category
            password.Append(validChars[random.Next(0, 26)]); // lowercase
            password.Append(validChars[random.Next(26, 52)]); // uppercase
            password.Append(validChars[random.Next(52, 62)]); // digit
            password.Append(validChars[random.Next(62, validChars.Length)]); // special

            // Fill the rest randomly
            for (int i = 4; i < length; i++)
            {
                password.Append(validChars[random.Next(validChars.Length)]);
            }

            // Shuffle the password
            var passwordArray = password.ToString().ToCharArray();
            for (int i = passwordArray.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (passwordArray[i], passwordArray[j]) = (passwordArray[j], passwordArray[i]);
            }

            return new string(passwordArray);
        }
    }
} 