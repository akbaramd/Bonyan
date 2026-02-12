using System.Security.Claims;
using Bonyan.IdentityManagement.BonWeb.Mvc.Models.Account;
using Bonyan.IdentityManagement.ClaimProvider;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;

namespace Bonyan.IdentityManagement.BonWeb.Mvc.Controllers.IdentityManagement;

[Area("IdentityManagement")]
[AllowAnonymous]
[Route("IdentityManagement/[controller]")]
public class AccountController : Controller
{
    public const string PasswordResetTokenType = "PasswordReset";
    private static readonly TimeSpan PasswordResetTokenExpiration = TimeSpan.FromHours(1);

    private readonly IBonIdentityUserManager _userManager;
    private readonly IBonIdentityClaimProviderManager _claimProviderManager;
    private readonly IWebHostEnvironment _env;
    private readonly IStringLocalizer<IdentityManagementResource> _localizer;

    public AccountController(
        IBonIdentityUserManager userManager,
        IBonIdentityClaimProviderManager claimProviderManager,
        IWebHostEnvironment env,
        IStringLocalizer<IdentityManagementResource> localizer)
    {
        _userManager = userManager;
        _claimProviderManager = claimProviderManager;
        _env = env;
        _localizer = localizer;
    }

    [HttpGet]
    [Route("[action]")]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");
        return View(new LoginInputModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("AuthEndpoints")]
    [Route("[action]")]
    public async Task<IActionResult> Login(LoginInputModel model, string? returnUrl = null, CancellationToken cancellationToken = default)
    {
        returnUrl ??= Url.Content("~/");
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var findResult = await _userManager.FindByUserNameAsync(model.UserName!);
        if (!findResult.IsSuccess || findResult.Value == null)
        {
            ModelState.AddModelError(string.Empty, _localizer["Validation:InvalidUserNameOrPassword"].Value);
            return View(model);
        }

        var user = findResult.Value;

        if (user.IsAccountLocked())
        {
            var lockoutEnd = user.AccountLockedUntil!.Value;
            var remainingMins = Math.Max(1, (int)Math.Ceiling((lockoutEnd - DateTime.Now).TotalMinutes));
            var message = string.Format(_localizer["Validation:AccountLocked"].Value, remainingMins);
            ModelState.AddModelError(string.Empty, message);
            return View(model);
        }

        if (!user.VerifyPassword(model.Password!))
        {
            user.IncrementFailedLoginAttemptCount();
            await _userManager.UpdateAsync(user);
            ModelState.AddModelError(string.Empty, _localizer["Validation:InvalidUserNameOrPassword"].Value);
            return View(model);
        }

        if (user.IsDeleted || user.Status != UserStatus.Active)
        {
            ModelState.AddModelError(string.Empty, _localizer["Validation:AccountInactive"].Value);
            return View(model);
        }

        user.ResetFailedLoginAttemptCount();
        await _userManager.UpdateAsync(user);

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
    [Route("[action]")]
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordInputModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("AuthEndpoints")]
    [Route("[action]")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordInputModel model, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return View(model);

        BonIdentityUser? user = null;
        var byUser = await _userManager.FindByUserNameAsync(model.UserNameOrEmail!);
        if (byUser.IsSuccess && byUser.Value != null)
            user = byUser.Value;
        if (user == null)
        {
            var byEmail = await _userManager.FindByEmailAsync(model.UserNameOrEmail!);
            if (byEmail.IsSuccess && byEmail.Value != null)
                user = byEmail.Value;
        }

        if (user == null)
        {
            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        var token = Guid.NewGuid().ToString("N");
        var expiration = DateTime.UtcNow.Add(PasswordResetTokenExpiration);
        var setResult = await _userManager.SetTokenAsync(user, PasswordResetTokenType, token, expiration);
        if (!setResult.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, _localizer["Validation:CouldNotGenerateResetLink"].Value);
            return View(model);
        }

        var resetLink = Url.Action(
            nameof(ResetPassword),
            "Account",
            new { area = "IdentityManagement", token },
            Request.Scheme);

        return RedirectToAction(nameof(ForgotPasswordConfirmation), new { resetLink = _env.IsDevelopment() ? resetLink : null });
    }

    [HttpGet]
    [Route("[action]")]
    public IActionResult ForgotPasswordConfirmation([FromQuery] string? resetLink = null)
    {
        ViewData["ResetLink"] = resetLink;
        return View();
    }

    [HttpGet]
    [Route("[action]")]
    public IActionResult ResetPassword(string? token = null)
    {
        if (string.IsNullOrEmpty(token))
            return RedirectToAction(nameof(Login));
        return View(new ResetPasswordInputModel { Token = token });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("[action]")]
    public async Task<IActionResult> ResetPassword(ResetPasswordInputModel model, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(model.Token))
        {
            return RedirectToAction(nameof(Login));
        }

        if (!ModelState.IsValid)
            return View(model);

        var findResult = await _userManager.FindByTokenAsync(PasswordResetTokenType, model.Token);
        if (!findResult.IsSuccess || findResult.Value == null)
        {
            ModelState.AddModelError(string.Empty, _localizer["Validation:InvalidOrExpiredResetLink"].Value);
            return View(model);
        }

        var user = findResult.Value;
        var resetResult = await _userManager.ResetPasswordAsync(user, model.NewPassword!);
        if (!resetResult.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, resetResult.ErrorMessage ?? _localizer["Validation:CouldNotResetPassword"].Value);
            return View(model);
        }

        await _userManager.RemoveTokenAsync(user, PasswordResetTokenType);
        return RedirectToAction(nameof(ResetPasswordConfirmation));
    }

    [HttpGet]
    [Route("[action]")]
    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [Route("[action]")]
    public async Task<IActionResult> Logout(string? returnUrl = null, CancellationToken cancellationToken = default)
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return LocalRedirect(returnUrl ?? Url.Content("~/"));
    }

    [HttpGet]
    [Route("[action]")]
    public IActionResult AccessDenied(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }
}
