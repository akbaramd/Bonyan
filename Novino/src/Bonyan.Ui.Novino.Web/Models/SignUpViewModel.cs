using System.ComponentModel.DataAnnotations;

namespace Bonyan.Novino.Web.Models
{
    /// <summary>
    /// View model for user registration
    /// </summary>
    public class SignUpViewModel
    {
        /// <summary>
        /// User's email address
        /// </summary>
        [Required(ErrorMessage = "ایمیل الزامی است")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل صحیح نیست")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Username for the account
        /// </summary>
        [Required(ErrorMessage = "نام کاربری الزامی است")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "نام کاربری باید بین ۳ تا ۱۰۰ کاراکتر باشد")]
        [Display(Name = "نام کاربری")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Password for the account
        /// </summary>
        [Required(ErrorMessage = "رمز عبور الزامی است")]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "رمز عبور باید حداقل ۸ کاراکتر باشد")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", 
            ErrorMessage = "رمز عبور باید شامل حروف کوچک، بزرگ و اعداد باشد")]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Password confirmation
        /// </summary>
        [Required(ErrorMessage = "تأیید رمز عبور الزامی است")]
        [Compare("Password", ErrorMessage = "رمز عبور و تأیید آن مطابقت ندارند")]
        [Display(Name = "تأیید رمز عبور")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// User's first name
        /// </summary>
        [Required(ErrorMessage = "نام الزامی است")]
        [StringLength(50, ErrorMessage = "نام نمی‌تواند بیش از ۵۰ کاراکتر باشد")]
        [Display(Name = "نام")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// User's last name
        /// </summary>
        [Required(ErrorMessage = "نام خانوادگی الزامی است")]
        [StringLength(50, ErrorMessage = "نام خانوادگی نمی‌تواند بیش از ۵۰ کاراکتر باشد")]
        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// User's phone number
        /// </summary>
        [Phone(ErrorMessage = "فرمت شماره تلفن صحیح نیست")]
        [Display(Name = "شماره تلفن")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Terms and conditions acceptance
        /// </summary>
        [Required(ErrorMessage = "پذیرش قوانین و مقررات الزامی است")]
        [Display(Name = "قوانین و مقررات")]
        public bool AcceptTerms { get; set; }

        /// <summary>
        /// Return URL after successful registration
        /// </summary>
        public string? ReturnUrl { get; set; }
    }
} 