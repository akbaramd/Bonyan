using System.ComponentModel.DataAnnotations;

namespace Bonyan.IdentityManagement.BonWeb.Mvc.Models.Account;

public class LoginInputModel
{
    [Required(ErrorMessage = "Validation:UserNameRequired")]
    [Display(Name = "Account:UserName")]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "Validation:PasswordRequired")]
    [Display(Name = "Account:Password")]
    public string? Password { get; set; }

    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}

public class ForgotPasswordInputModel
{
    [Required(ErrorMessage = "Validation:UserNameOrEmailRequired")]
    [Display(Name = "Account:UserNameOrEmail")]
    public string? UserNameOrEmail { get; set; }
}

public class ResetPasswordInputModel
{
    public string? Token { get; set; }

    [Required(ErrorMessage = "Validation:PasswordRequired")]
    [MinLength(6, ErrorMessage = "Validation:PasswordMinLength")]
    [Display(Name = "Account:NewPassword")]
    public string? NewPassword { get; set; }

    [Required(ErrorMessage = "Validation:PasswordRequired")]
    [Display(Name = "Account:ConfirmPassword")]
    [Compare(nameof(NewPassword), ErrorMessage = "Validation:PasswordConfirmationMismatch")]
    public string? ConfirmPassword { get; set; }
}
