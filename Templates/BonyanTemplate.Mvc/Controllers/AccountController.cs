using System.Security.Claims;
using Bonyan.IdentityManagement.ClaimProvider;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BonyanTemplate.Mvc.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    public const string PasswordResetTokenType = "PasswordReset";
    private static readonly TimeSpan PasswordResetTokenExpiration = TimeSpan.FromHours(1);

    private readonly IBonIdentityUserManager _userManager;
    private readonly IBonIdentityClaimProviderManager _claimProviderManager;
    private readonly IWebHostEnvironment _env;

    public AccountController(
        IBonIdentityUserManager userManager,
        IBonIdentityClaimProviderManager claimProviderManager,
        IWebHostEnvironment env)
    {
        _userManager = userManager;
        _claimProviderManager = claimProviderManager;
        _env = env;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");
        return View(new LoginInputModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginInputModel model, string? returnUrl = null, CancellationToken cancellationToken = default)
    {
        returnUrl ??= Url.Content("~/");
        ViewData["ReturnUrl"] = returnUrl;

        if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
        {
            ModelState.AddModelError(string.Empty, "User name and password are required.");
            return View(model);
        }

        var findResult = await _userManager.FindByUserNameAsync(model.UserName);
        if (!findResult.IsSuccess || findResult.Value == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid user name or password.");
            return View(model);
        }

        var user = findResult.Value;
        if (!user.VerifyPassword(model.Password))
        {
            ModelState.AddModelError(string.Empty, "Invalid user name or password.");
            return View(model);
        }

        var claims = await _claimProviderManager.GetClaimsAsync(user);
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var props = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(14) : null,
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);
        return LocalRedirect(returnUrl);
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordInputModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordInputModel model, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(model.UserNameOrEmail))
        {
            ModelState.AddModelError(string.Empty, "Please enter your user name or email.");
            return View(model);
        }

        BonIdentityUser? user = null;
        var byUser = await _userManager.FindByUserNameAsync(model.UserNameOrEmail);
        if (byUser.IsSuccess && byUser.Value != null)
            user = byUser.Value;
        if (user == null)
        {
            var byEmail = await _userManager.FindByEmailAsync(model.UserNameOrEmail);
            if (byEmail.IsSuccess && byEmail.Value != null)
                user = byEmail.Value;
        }

        if (user == null)
        {
            // Don't reveal that the user doesn't exist
            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        var token = Guid.NewGuid().ToString("N");
        var expiration = DateTime.UtcNow.Add(PasswordResetTokenExpiration);
        var setResult = await _userManager.SetTokenAsync(user, PasswordResetTokenType, token, expiration);
        if (!setResult.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, "Could not generate reset link. Please try again.");
            return View(model);
        }

        var resetLink = Url.Action(
            nameof(ResetPassword),
            "Account",
            new { token },
            Request.Scheme);

        return RedirectToAction(nameof(ForgotPasswordConfirmation), new { resetLink = _env.IsDevelopment() ? resetLink : null });
    }

    [HttpGet]
    public IActionResult ForgotPasswordConfirmation([FromQuery] string? resetLink = null)
    {
        ViewData["ResetLink"] = resetLink;
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword(string? token = null)
    {
        if (string.IsNullOrEmpty(token))
            return RedirectToAction(nameof(Login));
        return View(new ResetPasswordInputModel { Token = token });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordInputModel model, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(model.Token))
        {
            return RedirectToAction(nameof(Login));
        }

        if (string.IsNullOrWhiteSpace(model.NewPassword) || model.NewPassword.Length < 6)
        {
            ModelState.AddModelError(nameof(model.NewPassword), "Password must be at least 6 characters.");
            return View(model);
        }

        if (model.NewPassword != model.ConfirmPassword)
        {
            ModelState.AddModelError(nameof(model.ConfirmPassword), "Passwords do not match.");
            return View(model);
        }

        var findResult = await _userManager.FindByTokenAsync(PasswordResetTokenType, model.Token);
        if (!findResult.IsSuccess || findResult.Value == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid or expired reset link. Please request a new one.");
            return View(model);
        }

        var user = findResult.Value;
        var resetResult = await _userManager.ResetPasswordAsync(user, model.NewPassword);
        if (!resetResult.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, resetResult.ErrorMessage ?? "Could not reset password.");
            return View(model);
        }

        await _userManager.RemoveTokenAsync(user, PasswordResetTokenType);
        return RedirectToAction(nameof(ResetPasswordConfirmation));
    }

    [HttpGet]
    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout(string? returnUrl = null, CancellationToken cancellationToken = default)
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return LocalRedirect(returnUrl ?? Url.Content("~/"));
    }

    [HttpGet]
    public IActionResult AccessDenied(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }
}

public class LoginInputModel
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}

public class ForgotPasswordInputModel
{
    public string? UserNameOrEmail { get; set; }
}

public class ResetPasswordInputModel
{
    public string? Token { get; set; }
    public string? NewPassword { get; set; }
    public string? ConfirmPassword { get; set; }
}
