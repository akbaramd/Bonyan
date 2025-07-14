using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.IdentityManagement.Domain.Users.ValueObjects;
using Bonyan.Novino.Web.Models;
using Bonyan.UnitOfWork;
using Bonyan.UserManagement.Domain.Users;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Bonyan.Novino.Web.Services
{
    public class UserSeedingService
    {
        private readonly IBonIdentityUserManager<Models.User, Models.Role> _userManager;
        private readonly ILogger<UserSeedingService> _logger;
        private readonly IBonUnitOfWorkManager _unitOfWorkManager;

        public UserSeedingService(
            IBonIdentityUserManager<Models.User, Models.Role> userManager,
            ILogger<UserSeedingService> logger, IBonUnitOfWorkManager unitOfWorkManager)
        {
            _userManager = userManager;
            _logger = logger;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task SeedDefaultUsersAsync()
        {
            using var uow = _unitOfWorkManager.Begin();
            
            try
            {
                // Check if admin user exists
                var adminResult = await _userManager.FindByUserNameAsync("admin");
                if (!adminResult.IsSuccess || adminResult.Value == null)
                {
                                    // Create admin user profile
                var adminProfile = new UserProfile("Admin", "User", DateTime.Now.AddYears(-29), "Administrator");
                
                // Create admin user
                var adminUser = new Models.User(
                    BonUserId.NewId(),
                    "admin",
                    adminProfile
                );
                
                // Set email
                adminUser.SetEmail(new BonUserEmail("admin@bonyan.com"));
                
                // Set status
                adminUser.ChangeStatus(UserStatus.Active);

                    var createResult = await _userManager.CreateAsync(adminUser, "admin123");
                    if (createResult.IsSuccess)
                    {
                        _logger.LogInformation("Default admin user created successfully");
                    }
                    else
                    {
                        _logger.LogError("Failed to create admin user: {Error}", createResult.ErrorMessage);
                    }
                }
                else
                {
                    _logger.LogInformation("Admin user already exists");
                }

                await uow.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding default users");
                await uow.RollbackAsync();
            }
        }
    }
} 