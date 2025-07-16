using System.Security.Claims;
using System.Text.Json;
using Bonyan.IdentityManagement.Domain.Roles.DomainServices;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.IdentityManagement.Domain.Users.ValueObjects;
using Bonyan.IdentityManagement.Permissions;
using Bonyan.Layer.Domain.DomainService;
using Bonyan.Mediators;
using Bonyan.Module.NotificationManagement.Abstractions.Types;
using Bonyan.Module.NotificationManagement.Application.Commands;
using Bonyan.Novino.Domain.Entities;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Bonyan.Novino.Web.Models;
using Bonyan.Novino.Web.Services;

namespace Bonyan.Novino.Web.Controllers
{
    /// <summary>
    /// Handles user authentication and account management
    /// </summary>
    public class AccountController : Controller
    {
        private readonly IBonIdentityUserManager<Domain.Entities.User, Role> _userManager;
        private readonly IBonIdentityRoleManager<Role> _roleManager;
        private readonly IBonPermissionManager<Domain.Entities.User, Role> _permissionManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IBonMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Security configuration
        private const int MaxFailedLoginAttempts = 5;
        private const int LockoutDurationMinutes = 15;
        private const int SessionTimeoutHours = 8;
        private const int RememberMeDays = 30;

        public AccountController(
            IBonIdentityUserManager<Domain.Entities.User, Role> userManager,
            IBonIdentityRoleManager<Role> roleManager,
            IBonPermissionManager<Domain.Entities.User, Role> permissionManager,
            ILogger<AccountController> logger,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor, IBonMediator mediator)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _permissionManager = permissionManager ?? throw new ArgumentNullException(nameof(permissionManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mediator = mediator;
        }

        /// <summary>
        /// Displays the login form
        /// </summary>
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            // Prevent open redirect attacks
            if (!string.IsNullOrEmpty(returnUrl) && !Url.IsLocalUrl(returnUrl))
            {
                returnUrl = null;
            }

            // If user is already authenticated, redirect to dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                _logger.LogInformation("Authenticated user {Username} attempted to access login page, redirecting to dashboard", User.Identity.Name);
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Title"] = "Sign In";
            
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        /// <summary>
        /// Handles user login authentication
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            // Prevent open redirect attacks
            if (!string.IsNullOrEmpty(returnUrl) && !Url.IsLocalUrl(returnUrl))
            {
                returnUrl = null;
            }

            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Title"] = "Sign In";

            // Validate model
            if (!ModelState.IsValid)
            {
                LogValidationErrors(ModelState);
                return View(model);
            }

            // Sanitize input
            model.Username = model.Username?.Trim();
            model.Password = model.Password?.Trim();

            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError(string.Empty, "Username and password are required.");
                return View(model);
            }

            try
            {
                // Find user by username or email
                var userResult = await FindUserAsync(model.Username);
                if (!userResult.IsSuccess || userResult.Value == null)
                {
                    await LogFailedLoginAttemptAsync(model.Username, "User not found");
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                    return View(model);
                }

                var user = userResult.Value;

                // Check account status
                var accountStatusResult = await ValidateAccountStatusAsync(user);
                if (!accountStatusResult.IsSuccess)
                {
                    ModelState.AddModelError(string.Empty, accountStatusResult.ErrorMessage);
                    return View(model);
                }

                // Verify password with account lockout protection
                var passwordResult = await VerifyPasswordWithLockoutAsync(user, model.Password);
                if (!passwordResult.IsSuccess)
                {
                    ModelState.AddModelError(string.Empty, passwordResult.ErrorMessage);
                    return View(model);
                }

                // Reset failed login attempts on successful login
                await ResetFailedLoginAttemptsAsync(user);

                // Create authentication session
                var authResult = await CreateAuthenticationSessionAsync(user, model.RememberMe, returnUrl);
                if (!authResult.IsSuccess)
                {
                    ModelState.AddModelError(string.Empty, authResult.ErrorMessage);
                    return View(model);
                }

                // Log successful login
                await LogSuccessfulLoginAsync(user);

                // Redirect to return URL or dashboard
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                await _mediator.SendAsync(new SendNotificationCommand(NotificationChannel.InApp, user.Id.ToString(),
                    "ورود به سامانه", "", Purpose: "Account/Login"));
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login for user {Username}", model.Username);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                return View(model);
            }
        }

        /// <summary>
        /// Displays logout confirmation page
        /// </summary>
        [HttpGet]
        public IActionResult Logout(string returnUrl = null)
        {
            // Prevent open redirect attacks
            if (!string.IsNullOrEmpty(returnUrl) && !Url.IsLocalUrl(returnUrl))
            {
                returnUrl = null;
            }

            // If user is not authenticated, redirect to login
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login");
            }

            var model = new LogoutViewModel
            {
                ReturnUrl = returnUrl ?? "/",
                UserName = User.Identity.Name ?? "کاربر ناشناس"
            };

            return View(model);
        }

        /// <summary>
        /// Handles user logout confirmation
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutViewModel model)
        {
            try
            {
                var username = User.Identity?.Name;
                
                // Sign out the user
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                
                // Clear any session data
                HttpContext.Session.Clear();
                
                _logger.LogInformation("User {Username} logged out successfully", username);
                
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return RedirectToAction("Login");
            }
        }

        /// <summary>
        /// Displays access denied page
        /// </summary>
        [HttpGet]
        public IActionResult AccessDenied()
        {
            ViewData["Title"] = "Access Denied";
            return View();
        }

        /// <summary>
        /// Displays account locked page
        /// </summary>
        [HttpGet]
        public IActionResult AccountLocked()
        {
            ViewData["Title"] = "Account Locked";
            return View();
        }

        /// <summary>
        /// Displays password reset form
        /// </summary>
        [HttpGet]
        public IActionResult PasswordReset()
        {
            // If user is already authenticated, redirect to dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                _logger.LogInformation("Authenticated user {Username} attempted to access password reset page, redirecting to dashboard", User.Identity.Name);
                return RedirectToAction("Index", "Home");
            }

            ViewData["Title"] = "بازیابی رمز عبور";
            return View();
        }

        /// <summary>
        /// Handles password reset request
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PasswordReset(string email)
        {
            ViewData["Title"] = "بازیابی رمز عبور";

            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(string.Empty, "ایمیل الزامی است.");
                return View();
            }

            try
            {
                // Find user by email
                var userResult = await FindUserAsync(email);
                if (!userResult.IsSuccess || userResult.Value == null)
                {
                    // Don't reveal if email exists or not for security
                    _logger.LogInformation("Password reset requested for non-existent email: {Email}", email);
                    return RedirectToAction("PasswordResetConfirmation");
                }

                var user = userResult.Value;

                // TODO: Implement password reset logic
                // This would typically involve:
                // 1. Generating a reset token
                // 2. Sending email with reset link
                // 3. Storing reset token with expiration

                _logger.LogInformation("Password reset requested for user {Username}", user.UserName);

                return RedirectToAction("PasswordResetConfirmation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset for email {Email}", email);
                ModelState.AddModelError(string.Empty, "خطایی رخ داد. لطفاً دوباره تلاش کنید.");
                return View();
            }
        }

        /// <summary>
        /// Displays password reset confirmation page
        /// </summary>
        [HttpGet]
        public IActionResult PasswordResetConfirmation()
        {
            ViewData["Title"] = "تأیید درخواست بازیابی رمز عبور";
            return View();
        }

   



        #region Private Methods

        /// <summary>
        /// Finds user by username or email
        /// </summary>
        private async Task<BonDomainResult<Domain.Entities.User>> FindUserAsync(string usernameOrEmail)
        {
            // Try to find by username first
            var userResult = await _userManager.FindByUserNameAsync(usernameOrEmail);
            if (userResult.IsSuccess && userResult.Value != null)
            {
                return userResult;
            }

            // Try to find by email
            userResult = await _userManager.FindByEmailAsync(usernameOrEmail);
            return userResult;
        }

        /// <summary>
        /// Validates account status before login
        /// </summary>
        private async Task<LoginResult> ValidateAccountStatusAsync(Domain.Entities.User user)
        {
            // Check if account is locked
            if (user.IsAccountLocked())
            {
                var lockoutTime = user.AccountLockedUntil?.ToString("yyyy-MM-dd HH:mm:ss") ?? "unknown";
                _logger.LogWarning("Login attempt for locked account {Username}, locked until {LockoutTime}", 
                    user.UserName, lockoutTime);
                return LoginResult.Failure("Account is temporarily locked due to multiple failed login attempts. Please try again later.");
            }

            // Check user status
            return user.Status.Id switch
            {
                UserStatus.IdActive => LoginResult.Success(),
                UserStatus.IdInactive => LoginResult.Failure("Account is inactive. Please contact administrator."),
                UserStatus.IdSuspended => LoginResult.Failure("Account has been suspended. Please contact administrator."),
                UserStatus.IdDeactivated => LoginResult.Failure("Account is deactivated. Please contact administrator."),
                UserStatus.IdBanned => LoginResult.Failure("Account is locked. Please contact administrator."),
                UserStatus.IdPendingDeletion => LoginResult.Failure("Account is pending deletion."),
                UserStatus.IdPendingApproval => LoginResult.Failure("Account is pending approval. Please contact administrator."),
                UserStatus.IdArchived => LoginResult.Failure("Account has been archived."),
                UserStatus.IdPendingVerification => LoginResult.Failure("Account is pending verification. Please check your email."),
                _ => LoginResult.Failure("Account status is invalid. Please contact administrator.")
            };
                   
        }

        /// <summary>
        /// Verifies password with account lockout protection
        /// </summary>
        private async Task<LoginResult> VerifyPasswordWithLockoutAsync(Domain.Entities.User user, string password)
        {
            // Check if password is correct
            if (!user.VerifyPassword(password))
            {
                // Increment failed login attempts
                user.IncrementFailedLoginAttemptCount();
                await _userManager.UpdateAsync(user);

                var remainingAttempts = MaxFailedLoginAttempts - user.FailedLoginAttemptCount;
                
                if (remainingAttempts > 0)
                {
                    _logger.LogWarning("Failed login attempt for user {Username}, {RemainingAttempts} attempts remaining", 
                        user.UserName, remainingAttempts);
                    return LoginResult.Failure($"Invalid username or password. {remainingAttempts} attempts remaining.");
                }
                else
                {
                    // Lock the account
                    var lockoutUntil = DateTime.UtcNow.AddMinutes(LockoutDurationMinutes);
                    user.LockAccountUntil(lockoutUntil);
                    await _userManager.UpdateAsync(user);

                    _logger.LogWarning("Account locked for user {Username} due to multiple failed login attempts, locked until {LockoutTime}", 
                        user.UserName, lockoutUntil.ToString("yyyy-MM-dd HH:mm:ss"));
                    return LoginResult.Failure($"Account has been locked due to multiple failed login attempts. Please try again after {LockoutDurationMinutes} minutes.");
                }
            }

            return LoginResult.Success();
        }

        /// <summary>
        /// Resets failed login attempts on successful login
        /// </summary>
        private async Task ResetFailedLoginAttemptsAsync(Domain.Entities.User user)
        {
            if (user.FailedLoginAttemptCount > 0)
            {
                user.ResetFailedLoginAttemptCount();
                await _userManager.UpdateAsync(user);
                _logger.LogInformation("Reset failed login attempts for user {Username}", user.UserName);
            }
        }

        /// <summary>
        /// Creates authentication session with claims
        /// </summary>
        private async Task<LoginResult> CreateAuthenticationSessionAsync(Domain.Entities.User user, bool rememberMe, string returnUrl)
        {
            try
            {
                // Create claims for the user
                var claims = await CreateUserClaimsAsync(user);

                // Create authentication properties
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = rememberMe,
                    ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(RememberMeDays) : DateTimeOffset.UtcNow.AddHours(SessionTimeoutHours),
                    AllowRefresh = true,
                    RedirectUri = returnUrl
                };

                // Sign in the user
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                    authProperties);

                return LoginResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating authentication session for user {Username}", user.UserName);
                return LoginResult.Failure("Error creating authentication session. Please try again.");
            }
        }

        /// <summary>
        /// Creates user claims for authentication
        /// </summary>
        private async Task<List<Claim>> CreateUserClaimsAsync(Domain.Entities.User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email?.Address ?? ""),
                new Claim("UserId", user.Id.ToString()),
                new Claim("UserName", user.UserName),
                new Claim("UserStatus", user.Status.ToString()),
                new Claim("LoginTime", DateTime.UtcNow.ToString("O")),
                new Claim("SessionId", Guid.NewGuid().ToString())
            };

            // Add profile information
            if (user.Profile != null)
            {
                claims.Add(new Claim("FirstName", user.Profile.FirstName ?? ""));
                claims.Add(new Claim("LastName", user.Profile.LastName ?? ""));
                claims.Add(new Claim("DisplayName", $"{user.Profile.FirstName} {user.Profile.LastName}" ?? user.UserName));
            }

            // Get user roles
            var rolesResult = await _userManager.GetUserRolesAsync(user);
            if (rolesResult.IsSuccess)
            {
                foreach (var role in rolesResult.Value)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Title));
                    claims.Add(new Claim("RoleId", role.Id.ToString()));
                }
            }

            // Get user permissions
            try
            {
                var permissions = await _permissionManager.GetUserPermissionsAsync(user.Id);
                foreach (var permission in permissions)
                {
                    claims.Add(new Claim("Permission", permission));
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error retrieving permissions for user {Username}", user.UserName);
            }

            // Add custom claims
            var userClaimsResult = await _userManager.GetAllClaimsAsync(user);
            if (userClaimsResult.IsSuccess)
            {
                foreach (var claim in userClaimsResult.Value)
                {
                    claims.Add(new Claim(claim.ClaimType, claim.ClaimValue, claim.ClaimValueType, claim.Issuer));
                }
            }

            return claims;
        }

        /// <summary>
        /// Logs successful login
        /// </summary>
        private async Task LogSuccessfulLoginAsync(Domain.Entities.User user)
        {
            var loginInfo = new
            {
                UserId = user.Id.ToString(),
                Username = user.UserName,
                LoginTime = DateTime.UtcNow,
                IpAddress = GetClientIpAddress(),
                UserAgent = GetUserAgent(),
                SessionId = Guid.NewGuid().ToString()
            };

            _logger.LogInformation("User {Username} logged in successfully. Login Info: {@LoginInfo}", 
                user.UserName, loginInfo);
        }

        /// <summary>
        /// Logs failed login attempt
        /// </summary>
        private async Task LogFailedLoginAttemptAsync(string username, string reason)
        {
            var loginAttempt = new
            {
                Username = username,
                AttemptTime = DateTime.UtcNow,
                IpAddress = GetClientIpAddress(),
                UserAgent = GetUserAgent(),
                Reason = reason
            };

            _logger.LogWarning("Failed login attempt. Attempt Info: {@LoginAttempt}", loginAttempt);
        }

        /// <summary>
        /// Logs validation errors
        /// </summary>
        private void LogValidationErrors(ModelStateDictionary modelState)
        {
            var errors = modelState
                .Where(x => x.Value?.Errors.Count > 0)
                .Select(x => new { Field = x.Key, Errors = x.Value?.Errors.Select(e => e.ErrorMessage) })
                .ToList();

            if (errors.Any())
            {
                _logger.LogWarning("Model validation errors: {@ValidationErrors}", errors);
            }
        }

        /// <summary>
        /// Gets client IP address
        /// </summary>
        private string GetClientIpAddress()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return "Unknown";

            // Check for forwarded headers (for proxy/load balancer scenarios)
            var forwardedHeader = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedHeader))
            {
                return forwardedHeader.Split(',')[0].Trim();
            }

            return httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        /// <summary>
        /// Gets user agent
        /// </summary>
        private string GetUserAgent()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext?.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown";
        }

        #endregion
    }

    /// <summary>
    /// Result of login operation
    /// </summary>
    public class LoginResult
    {
        public bool IsSuccess { get; private set; }
        public string ErrorMessage { get; private set; }

        private LoginResult(bool isSuccess, string errorMessage = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public static LoginResult Success() => new LoginResult(true);
        public static LoginResult Failure(string errorMessage) => new LoginResult(false, errorMessage);
    }
} 
