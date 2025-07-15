using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Bonyan.Novino.Web.Models
{
    /// <summary>
    /// View model for user login
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Username or email address
        /// </summary>
        [Required(ErrorMessage = "Username or email is required")]
        [Display(Name = "Username or Email")]
        [StringLength(100, ErrorMessage = "Username or email cannot exceed {1} characters")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-@]+$", ErrorMessage = "Username or email contains invalid characters")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// User password
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "Password must be between {2} and {1} characters")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Remember me option
        /// </summary>
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        /// <summary>
        /// Return URL after successful login
        /// </summary>
        public string? ReturnUrl { get; set; }

        /// <summary>
        /// Captcha token for bot protection
        /// </summary>
        [Display(Name = "Security Verification")]
        public string? CaptchaToken { get; set; }

        /// <summary>
        /// Client-side validation summary
        /// </summary>
        public bool HasValidationErrors => !string.IsNullOrEmpty(ValidationSummary);

        /// <summary>
        /// Validation summary for display
        /// </summary>
        public string? ValidationSummary { get; set; }

        /// <summary>
        /// Sanitizes the input data
        /// </summary>
        public void Sanitize()
        {
            Username = Username?.Trim() ?? string.Empty;
            Password = Password?.Trim() ?? string.Empty;
            ReturnUrl = ReturnUrl?.Trim();
            CaptchaToken = CaptchaToken?.Trim();
        }

        /// <summary>
        /// Validates the model with custom business rules
        /// </summary>
        /// <returns>Validation result</returns>
        public ValidationResult Validate()
        {
            var errors = new List<string>();

            // Check for suspicious patterns
            if (IsSuspiciousInput(Username))
            {
                errors.Add("Username contains suspicious patterns");
            }

            if (IsSuspiciousInput(Password))
            {
                errors.Add("Password contains suspicious patterns");
            }

            // Check for common attack patterns
            if (ContainsSqlInjectionPatterns(Username) || ContainsSqlInjectionPatterns(Password))
            {
                errors.Add("Input contains invalid characters");
            }

            if (errors.Any())
            {
                ValidationSummary = string.Join("; ", errors);
                return ValidationResult.Failure(ValidationSummary);
            }

            return ValidationResult.Success();
        }

        /// <summary>
        /// Checks if input contains suspicious patterns
        /// </summary>
        private static bool IsSuspiciousInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            // Check for excessive special characters
            var specialCharCount = input.Count(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));
            if (specialCharCount > input.Length * 0.3) // More than 30% special characters
            {
                return true;
            }

            // Check for repeated characters
            if (input.Length > 3)
            {
                for (int i = 0; i <= input.Length - 3; i++)
                {
                    if (input[i] == input[i + 1] && input[i] == input[i + 2])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if input contains SQL injection patterns
        /// </summary>
        private static bool ContainsSqlInjectionPatterns(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            var sqlPatterns = new[]
            {
                @"(\b(SELECT|INSERT|UPDATE|DELETE|DROP|CREATE|ALTER|EXEC|EXECUTE|UNION|SCRIPT)\b)",
                @"(--|/\*|\*/|xp_|sp_)",
                @"(\b(OR|AND)\b\s+\d+\s*=\s*\d+)",
                @"(\b(OR|AND)\b\s+['""]\w+['""]\s*=\s*['""]\w+['""])",
                @"(;\s*$|;\s*--)",
                @"(\b(WAITFOR|DELAY)\b)",
                @"(\b(BENCHMARK|SLEEP)\b)",
                @"(\b(LOAD_FILE|INTO\s+OUTFILE)\b)"
            };

            return sqlPatterns.Any(pattern => Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase));
        }
    }

    /// <summary>
    /// Validation result for login model
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; private set; }
        public string? ErrorMessage { get; private set; }

        private ValidationResult(bool isValid, string? errorMessage = null)
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }

        public static ValidationResult Success() => new ValidationResult(true);
        public static ValidationResult Failure(string errorMessage) => new ValidationResult(false, errorMessage);
    }
} 