# Bonyan Admin Login System

A professional, production-ready authentication system for the Bonyan Admin Dashboard with advanced security features, comprehensive error handling, and excellent user experience.

## 🚀 Features

### Security Features
- **Account Lockout Protection**: Automatic account locking after 5 failed login attempts
- **Password Verification**: Secure password hashing and verification using PBKDF2
- **Input Validation**: Comprehensive validation with SQL injection and XSS protection
- **Security Headers**: X-Frame-Options, X-Content-Type-Options, X-XSS-Protection, CSP
- **Session Management**: Configurable session timeouts and remember me functionality
- **Audit Logging**: Detailed logging of login attempts, successes, and failures
- **IP Tracking**: Client IP address tracking for security monitoring

### User Experience
- **Responsive Design**: Mobile-first design with excellent UX across all devices
- **Accessibility**: WCAG 2.1 compliant with proper ARIA labels and keyboard navigation
- **Theme Support**: Light/dark theme with system preference detection
- **Real-time Validation**: Client-side validation with immediate feedback
- **Loading States**: Visual feedback during authentication process
- **Error Handling**: User-friendly error messages with actionable guidance

### Technical Features
- **Modular Architecture**: Clean separation of concerns with dependency injection
- **Async/Await**: Full asynchronous operation for optimal performance
- **Exception Handling**: Comprehensive error handling with graceful degradation
- **Logging**: Structured logging with correlation IDs and context
- **Configuration**: Environment-based configuration management
- **Testing Ready**: Designed for unit and integration testing

## 📁 File Structure

```
Bonyan.Novino.Web/
├── Controllers/
│   └── AccountController.cs          # Main authentication controller
├── Models/
│   └── LoginViewModel.cs             # Login form model with validation
├── Views/
│   └── Account/
│       ├── Login.cshtml              # Login page with enhanced UX
│       ├── AccessDenied.cshtml       # Access denied page
│       └── AccountLocked.cshtml      # Account locked page
├── Services/
│   └── UserSeedingService.cs         # Default user creation service
└── BonyanNovinoWebModule.cs          # Module configuration
```

## 🔧 Configuration

### Authentication Settings

The system is configured in `BonyanNovinoWebModule.cs`:

```csharp
// Security configuration constants
private const int MaxFailedLoginAttempts = 5;
private const int LockoutDurationMinutes = 15;
private const int SessionTimeoutHours = 8;
private const int RememberMeDays = 30;
```

### Cookie Authentication

```csharp
context.Services.Configure<CookieAuthenticationOptions>(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
});
```

## 🔐 Security Implementation

### Password Security

The system uses the built-in `BonUserPassword` class which implements:
- **PBKDF2 Hashing**: 10,000 iterations with 32-byte hash
- **Salt Generation**: 16-byte random salt per password
- **Secure Verification**: Constant-time comparison to prevent timing attacks

### Account Lockout

```csharp
// Automatic lockout after failed attempts
if (user.FailedLoginAttemptCount >= MaxFailedLoginAttempts)
{
    var lockoutUntil = DateTime.UtcNow.AddMinutes(LockoutDurationMinutes);
    user.LockAccountUntil(lockoutUntil);
    // Log and notify user
}
```

### Input Validation

The `LoginViewModel` includes comprehensive validation:

```csharp
[Required(ErrorMessage = "Username or email is required")]
[StringLength(100, ErrorMessage = "Username or email cannot exceed {1} characters")]
[RegularExpression(@"^[a-zA-Z0-9._%+-@]+$", ErrorMessage = "Username or email contains invalid characters")]
public string Username { get; set; }
```

### Security Headers

```csharp
Context.Response.Headers.Add("X-Frame-Options", "DENY");
Context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
Context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
Context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
Context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; ...");
```

## 🎨 User Interface

### Login Page Features

- **Modern Design**: Clean, professional interface with gradient backgrounds
- **Responsive Layout**: Optimized for desktop, tablet, and mobile devices
- **Accessibility**: Screen reader support, keyboard navigation, high contrast mode
- **Theme Support**: Automatic light/dark theme switching
- **Loading States**: Visual feedback during form submission
- **Error Display**: Clear, actionable error messages

### CSS Features

```css
/* CSS Custom Properties for theming */
:root {
    --primary-color: #4f46e5;
    --primary-dark: #3730a3;
    --success-color: #10b981;
    --warning-color: #f59e0b;
    --danger-color: #ef4444;
    /* ... more variables */
}

/* Dark theme support */
[data-bs-theme="dark"] {
    --text-primary: #f1f5f9;
    --text-secondary: #94a3b8;
    /* ... dark theme variables */
}
```

### JavaScript Features

- **Form Validation**: Real-time client-side validation
- **Security**: Input sanitization and pattern detection
- **UX Enhancements**: Auto-focus, keyboard navigation, loading states
- **Performance**: Resource preloading and optimization
- **Accessibility**: ARIA support and keyboard shortcuts

## 📊 Logging and Monitoring

### Structured Logging

```csharp
// Successful login
_logger.LogInformation("User {Username} logged in successfully. Login Info: {@LoginInfo}", 
    user.UserName, loginInfo);

// Failed login attempt
_logger.LogWarning("Failed login attempt. Attempt Info: {@LoginAttempt}", loginAttempt);

// Account lockout
_logger.LogWarning("Account locked for user {Username} due to multiple failed login attempts", 
    user.UserName);
```

### Audit Information

Each login attempt logs:
- User ID and username
- Timestamp
- IP address
- User agent
- Success/failure status
- Session ID
- Lockout information

## 🧪 Testing

### Unit Testing

The system is designed for comprehensive testing:

```csharp
// Example test structure
[Test]
public async Task Login_WithValidCredentials_ShouldAuthenticateUser()
{
    // Arrange
    var mockUserManager = new Mock<IBonIdentityUserManager<BonIdentityUser>>();
    var mockLogger = new Mock<ILogger<AccountController>>();
    
    // Act
    var controller = new AccountController(mockUserManager.Object, ...);
    var result = await controller.Login(validLoginModel);
    
    // Assert
    Assert.IsInstanceOf<RedirectResult>(result);
}
```

### Integration Testing

```csharp
[Test]
public async Task Login_WithInvalidCredentials_ShouldReturnError()
{
    // Test invalid credentials
    // Test account lockout
    // Test validation errors
}
```

## 🚀 Deployment

### Production Checklist

- [ ] Configure HTTPS and secure cookies
- [ ] Set up proper logging and monitoring
- [ ] Configure rate limiting
- [ ] Set up backup and recovery procedures
- [ ] Configure session storage (Redis recommended)
- [ ] Set up alerting for security events
- [ ] Configure CDN for static assets
- [ ] Set up health checks

### Environment Variables

```json
{
  "Authentication": {
    "MaxFailedAttempts": 5,
    "LockoutDurationMinutes": 15,
    "SessionTimeoutHours": 8,
    "RememberMeDays": 30
  },
  "Security": {
    "RequireHttps": true,
    "SecureCookies": true
  }
}
```

## 🔧 Customization

### Adding Custom Validation

```csharp
public class CustomLoginValidator : ILoginValidator
{
    public ValidationResult Validate(LoginViewModel model)
    {
        // Custom validation logic
        return ValidationResult.Success();
    }
}
```

### Custom Authentication Logic

```csharp
public class CustomAuthenticationService : IAuthenticationService
{
    public async Task<AuthenticationResult> AuthenticateAsync(string username, string password)
    {
        // Custom authentication logic
        return AuthenticationResult.Success();
    }
}
```

## 📚 API Reference

### AccountController Methods

- `Login(string returnUrl)` - Display login form
- `Login(LoginViewModel model, string returnUrl)` - Process login
- `Logout()` - Process logout
- `AccessDenied()` - Display access denied page
- `AccountLocked()` - Display account locked page

### LoginViewModel Properties

- `Username` - Username or email
- `Password` - User password
- `RememberMe` - Remember me option
- `ReturnUrl` - Return URL after login
- `CaptchaToken` - Security verification token

## 🐛 Troubleshooting

### Common Issues

1. **Account Locked**: Wait for lockout period or contact support
2. **Invalid Credentials**: Check username/password or reset password
3. **Session Expired**: Re-authenticate with valid credentials
4. **Access Denied**: Contact administrator for proper permissions

### Debug Information

Enable detailed logging in development:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Bonyan.Novino.Web.Controllers.AccountController": "Debug"
    }
  }
}
```

## 📄 License

This login system is part of the Bonyan Admin Dashboard and follows the same licensing terms.

## 🤝 Contributing

When contributing to the login system:

1. Follow security best practices
2. Add comprehensive tests
3. Update documentation
4. Follow the existing code style
5. Consider accessibility implications

---

**Note**: This login system is designed for production use with enterprise-grade security features. Always review and customize security settings according to your specific requirements and compliance needs. 