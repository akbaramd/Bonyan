using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.DomainServices;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.IdentityManagement.Domain.Users.ValueObjects;
using Bonyan.IdentityManagement.Permissions;
using Bonyan.Novino.Domain.Entities;
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
        private readonly IBonIdentityUserManager<Domain.Entities.User, Role> _userManager;
        private readonly IBonIdentityRoleManager<Role> _roleManager;
        private readonly IBonPermissionManager<Domain.Entities.User, Role> _permissionManager;
        private readonly ILogger<UserSeedingService> _logger;
        private readonly IBonUnitOfWorkManager _unitOfWorkManager;

        // Constants for seeded entities
        private const string ADMIN_ROLE_ID = "admin";
        private const string ADMIN_ROLE_TITLE = "مدیریت";
        private const string ADMIN_USERNAME = "admin";
        private const string DEVELOPER_USERNAME = "developer";

        public UserSeedingService(
            IBonIdentityUserManager<Domain.Entities.User, Role> userManager,
            IBonIdentityRoleManager<Role> roleManager,
            IBonPermissionManager<Domain.Entities.User, Role> permissionManager,
            ILogger<UserSeedingService> logger, 
            IBonUnitOfWorkManager unitOfWorkManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _permissionManager = permissionManager;
            _logger = logger;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task SeedDefaultUsersAsync()
        {
            using var uow = _unitOfWorkManager.Begin();
            
            try
            {
                _logger.LogInformation("Starting identity seeding process...");
                
                // Initialize permissions first
                _permissionManager.InitializePermissions();
                _logger.LogInformation("Permissions initialized successfully");
                
                // Create or update roles
                await CreateOrUpdateRolesAsync();
                
                // Create or update users
                await CreateOrUpdateAdminUserAsync();
                await CreateOrUpdateDeveloperUserAsync();

                // Synchronize permissions with database
                await SynchronizePermissionsAsync();

                await uow.CompleteAsync();
                _logger.LogInformation("Identity seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during identity seeding");
                await uow.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Creates or updates roles with proper synchronization
        /// </summary>
        private async Task CreateOrUpdateRolesAsync()
        {
            // Create or update Admin Role
            await CreateOrUpdateRoleAsync(ADMIN_ROLE_ID, ADMIN_ROLE_TITLE, false);
            
        }

        /// <summary>
        /// Creates or updates a specific role
        /// </summary>
        private async Task CreateOrUpdateRoleAsync(string roleId, string title, bool canBeDeleted)
        {
            try
            {
                var roleIdValue = BonRoleId.FromValue(roleId);
                
                // Check if role exists
                var existingRole = await _roleManager.FindRoleByIdAsync(roleId);
                
                if (existingRole.IsSuccess && existingRole.Value != null)
                {
                    // Role exists, update if needed
                    var role = existingRole.Value;
                    if (role.Title != title)
                    {
                        role.UpdateTitle(title);
                        await _roleManager.UpdateRoleAsync(role);
                        _logger.LogInformation("Role '{RoleId}' title updated to '{Title}'", roleId, title);
                    }
                    else
                    {
                        _logger.LogInformation("Role '{RoleId}' already exists with correct title", roleId);
                    }
                }
                else
                {
                    // Create new role
                    var newRole = new Role(roleIdValue, title, canBeDeleted);
                    var createResult = await _roleManager.CreateRoleAsync(newRole);
                    
                    if (createResult.IsSuccess)
                    {
                        _logger.LogInformation("Role '{RoleId}' created successfully with title '{Title}'", roleId, title);
                    }
                    else
                    {
                        _logger.LogError("Failed to create role '{RoleId}': {Error}", roleId, createResult.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating/updating role '{RoleId}'", roleId);
                throw;
            }
        }

        /// <summary>
        /// Creates or updates admin user
        /// </summary>
        private async Task CreateOrUpdateAdminUserAsync()
        {
            await CreateOrUpdateUserAsync(
                ADMIN_USERNAME,
                "مدیر سیستم",
                "کاربر",
                "admin@bonyan.com",
                "admin123",
                ADMIN_ROLE_ID,
                "مدیر ارشد سیستم با دسترسی کامل"
            );
        }

        /// <summary>
        /// Creates or updates developer user
        /// </summary>
        private async Task CreateOrUpdateDeveloperUserAsync()
        {
            await CreateOrUpdateUserAsync(
                DEVELOPER_USERNAME,
                "برنامه نویس",
                "کاربر",
                "developer@bonyan.com",
                "developer123",
                ADMIN_ROLE_ID,
                "برنامه نویس سیستم"
            );
        }

        /// <summary>
        /// Creates or updates a user with specified details
        /// </summary>
        private async Task CreateOrUpdateUserAsync(string username, string firstName, string lastName, 
            string email, string password, string roleId, string description)
        {
            try
            {
                // Check if user exists
                var existingUser = await _userManager.FindByUserNameAsync(username);
                
                if (existingUser.IsSuccess && existingUser.Value != null)
                {
                    // User exists, update if needed
                    var user = existingUser.Value;
                    var needsUpdate = false;
                    
                    // Check if email needs update
                    if (user.Email?.Address != email)
                    {
                        user.SetEmail(new BonUserEmail(email));
                        needsUpdate = true;
                    }
                    
                    // Check if profile needs update
                    if (user.Profile.FirstName != firstName || user.Profile.LastName != lastName)
                    {
                        var newProfile = new UserProfile(firstName, lastName, user.Profile.DateOfBirth, description);
                        // Note: You might need to implement a method to update profile
                        needsUpdate = true;
                    }
                    
                    if (needsUpdate)
                    {
                        await _userManager.UpdateAsync(user);
                        _logger.LogInformation("User '{Username}' updated successfully", username);
                    }
                    else
                    {
                        _logger.LogInformation("User '{Username}' already exists with correct details", username);
                    }
                }
                else
                {
                    // Create new user with active status from the start
                    var userProfile = new UserProfile(firstName, lastName, DateTime.Now.AddYears(-25), description);
                    var userId = BonUserId.NewId();
                    
                    var newUser = new Domain.Entities.User(userId, username, userProfile);
                    newUser.SetEmail(new BonUserEmail(email));
                    
                    // Note: UserStatus.Active is set by default in the BonUser constructor
                    // We don't need to explicitly call ChangeStatus here to avoid the EF issue
                    
                    var createResult = await _userManager.CreateAsync(newUser, password);
                    
                    if (createResult.IsSuccess)
                    {
                        _logger.LogInformation("User '{Username}' created successfully", username);
                        
                        // Assign role
                        await AssignRoleToUserAsync(newUser, roleId);
                    }
                    else
                    {
                        _logger.LogError("Failed to create user '{Username}': {Error}", username, createResult.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating/updating user '{Username}'", username);
                throw;
            }
        }

        /// <summary>
        /// Assigns a role to a user
        /// </summary>
        private async Task AssignRoleToUserAsync(Domain.Entities.User user, string roleId)
        {
            try
            {
                var role = await _roleManager.FindRoleByIdAsync(roleId);
                if (role.IsSuccess && role.Value != null)
                {
                    await _userManager.AssignRolesAsync(user, new[] { role.Value.Id });
                    _logger.LogInformation("Role '{RoleId}' assigned to user '{Username}'", roleId, user.UserName);
                }
                else
                {
                    _logger.LogWarning("Role '{RoleId}' not found for user '{Username}'", roleId, user.UserName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role '{RoleId}' to user '{Username}'", roleId, user.UserName);
            }
        }

        /// <summary>
        /// Synchronizes all permissions with the database
        /// </summary>
        private async Task SynchronizePermissionsAsync()
        {
            try
            {
                var allPermissions = _permissionManager.GetAllPermissions().ToList();
                _logger.LogInformation("Found {PermissionCount} permissions to synchronize", allPermissions.Count);
                
                // Grant all permissions to admin role
                await GrantAllPermissionsToRoleAsync(ADMIN_ROLE_ID, allPermissions);
                
                // Grant all permissions directly to admin user
                await GrantAllPermissionsToUserAsync(DEVELOPER_USERNAME, allPermissions);
                
                _logger.LogInformation("Permission synchronization completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during permission synchronization");
                throw;
            }
        }

        /// <summary>
        /// Grants all specified permissions to a role
        /// </summary>
        private async Task GrantAllPermissionsToRoleAsync(string roleId, IEnumerable<PermissionDefinition> permissions)
        {
            try
            {
                var roleIdValue = BonRoleId.FromValue(roleId);
                var permissionCount = 0;
                
                foreach (var permission in permissions)
                {
                    await _permissionManager.GrantPermissionToRoleAsync(roleIdValue, permission.Name);
                    permissionCount++;
                }
                
                _logger.LogInformation("Granted {PermissionCount} permissions to role '{RoleId}'", permissionCount, roleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error granting permissions to role '{RoleId}'", roleId);
                throw;
            }
        }

        /// <summary>
        /// Grants all specified permissions to a user
        /// </summary>
        private async Task GrantAllPermissionsToUserAsync(string username, IEnumerable<PermissionDefinition> permissions)
        {
            try
            {
                var user = await _userManager.FindByUserNameAsync(username);
                if (user.IsSuccess && user.Value != null)
                {
                    var permissionCount = 0;
                    
                    foreach (var permission in permissions)
                    {
                        await _permissionManager.GrantPermissionToUserAsync(user.Value.Id, permission.Name);
                        permissionCount++;
                    }
                    
                    _logger.LogInformation("Granted {PermissionCount} permissions directly to user '{Username}'", permissionCount, username);
                }
                else
                {
                    _logger.LogWarning("User '{Username}' not found for permission assignment", username);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error granting permissions to user '{Username}'", username);
                throw;
            }
        }

        /// <summary>
        /// Synchronizes roles and permissions when new permissions are added
        /// </summary>
        public async Task SynchronizeNewPermissionsAsync()
        {
            using var uow = _unitOfWorkManager.Begin();
            
            try
            {
                _logger.LogInformation("Starting permission synchronization for new permissions...");
                
                // Re-initialize permissions to get any new ones
                _permissionManager.InitializePermissions();
                
                // Get all current permissions
                var allPermissions = _permissionManager.GetAllPermissions().ToList();
                
                // Grant new permissions to admin role
                await GrantAllPermissionsToRoleAsync(ADMIN_ROLE_ID, allPermissions);
                
                // Grant new permissions to admin user
                await GrantAllPermissionsToUserAsync(ADMIN_USERNAME, allPermissions);
                
                await uow.CompleteAsync();
                _logger.LogInformation("New permission synchronization completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during new permission synchronization");
                await uow.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Removes a role and its permissions from the system
        /// </summary>
        public async Task RemoveRoleAsync(string roleId)
        {
            using var uow = _unitOfWorkManager.Begin();
            
            try
            {
                var role = await _roleManager.FindRoleByIdAsync(roleId);
                if (role.IsSuccess && role.Value != null)
                {
                    // Remove all permissions from the role first
                    var rolePermissions = await _permissionManager.GetRolePermissionsAsync(role.Value.Id);
                    foreach (var permission in rolePermissions)
                    {
                        await _permissionManager.RevokePermissionFromRoleAsync(role.Value.Id, permission);
                    }
                    
                    // Delete the role
                    await _roleManager.DeleteRoleAsync(role.Value);
                    
                    _logger.LogInformation("Role '{RoleId}' and its permissions removed successfully", roleId);
                }
                else
                {
                    _logger.LogWarning("Role '{RoleId}' not found for removal", roleId);
                }
                
                await uow.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing role '{RoleId}'", roleId);
                await uow.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Removes a permission from all roles and users
        /// </summary>
        public async Task RemovePermissionAsync(string permissionName)
        {
            using var uow = _unitOfWorkManager.Begin();
            
            try
            {
                _logger.LogInformation("Removing permission '{PermissionName}' from all roles and users", permissionName);
                
                // Get all roles and remove the permission
                // Note: You might need to implement a method to get all roles
                // For now, we'll remove from known roles
                var adminRole = await _roleManager.FindRoleByIdAsync(ADMIN_ROLE_ID);
                if (adminRole.IsSuccess && adminRole.Value != null)
                {
                    await _permissionManager.RevokePermissionFromRoleAsync(adminRole.Value.Id, permissionName);
                }
                
                var developerRole = await _roleManager.FindRoleByIdAsync(ADMIN_ROLE_ID);
                if (developerRole.IsSuccess && developerRole.Value != null)
                {
                    await _permissionManager.RevokePermissionFromRoleAsync(developerRole.Value.Id, permissionName);
                }
                
                // Remove from admin user
                var adminUser = await _userManager.FindByUserNameAsync(ADMIN_USERNAME);
                if (adminUser.IsSuccess && adminUser.Value != null)
                {
                    await _permissionManager.RevokePermissionFromUserAsync(adminUser.Value.Id, permissionName);
                }
                
                await uow.CompleteAsync();
                _logger.LogInformation("Permission '{PermissionName}' removed successfully", permissionName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing permission '{PermissionName}'", permissionName);
                await uow.RollbackAsync();
                throw;
            }
        }
    }
} 