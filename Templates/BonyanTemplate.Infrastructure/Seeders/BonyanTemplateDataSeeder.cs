﻿using System.Data;
using Bonyan.IdentityManagement;
using Bonyan.IdentityManagement.Domain.Permissions.ValueObjects;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.DomainServices;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.UnitOfWork;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using BonyanTemplate.Domain.Users;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BonyanTemplate.Infrastructure.Seeders;

public class BonyanTemplateDataSeeder : BackgroundService
{
    private readonly IBonIdentityRoleManager _bonIdentityRoleManager;
    private readonly IBonIdentityUserManager<User> _bonIdentityUserManager;
    private readonly ILogger<BonyanTemplateDataSeeder> _logger;
    private readonly IBonUnitOfWorkManager _uowManager;

    public BonyanTemplateDataSeeder(
        IBonIdentityRoleManager bonIdentityRoleManager,
        IBonIdentityUserManager<User> bonIdentityUserManager,
        ILogger<BonyanTemplateDataSeeder> logger, IBonUnitOfWorkManager uowManager)
    {
        _bonIdentityRoleManager = bonIdentityRoleManager;
        _bonIdentityUserManager = bonIdentityUserManager;
        _logger = logger;
        _uowManager = uowManager;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var uow = _uowManager.Begin(new BonUnitOfWorkOptions()
            {
                IsTransactional = true,
                IsolationLevel = IsolationLevel.ReadCommitted
            }); 
            
            _logger.LogInformation("Starting data seeding process...");

            // Create Admin Role
            var adminRoleId = BonRoleId.NewId("admin");
            var adminRole = BonIdentityRole.CreateNonDeletable(adminRoleId, "توسعه دهنده");

            var adminRoleCreatedResult = await _bonIdentityRoleManager.CreateRoleWithPermissionsAsync(adminRole, new[]
            {
                BonPermissionId.NewId(BonIdentityPermissionConstants.IdentityPermissionRead),
                BonPermissionId.NewId(BonIdentityPermissionConstants.IdentityRoleRead),
                BonPermissionId.NewId(BonIdentityPermissionConstants.IdentityRoleDelete),
                BonPermissionId.NewId(BonIdentityPermissionConstants.IdentityRoleEdit),
                BonPermissionId.NewId(BonIdentityPermissionConstants.IdentityRoleCreate),
                BonPermissionId.NewId(BonIdentityPermissionConstants.IdentityUserRead),
                BonPermissionId.NewId(BonIdentityPermissionConstants.IdentityUserDelete),
                BonPermissionId.NewId(BonIdentityPermissionConstants.IdentityUserEdit),
                BonPermissionId.NewId(BonIdentityPermissionConstants.IdentityUserCreate)
            });

            _logger.LogInformation("Admin role created successfully with ID: {AdminRoleId}", adminRoleId);

            // Create Admin User
            var adminUser = new User(BonUserId.NewId(), "admin");
            adminUser.SetPhoneNumber(new BonUserPhoneNumber("0987654321"));
            adminUser.SetEmail(new BonUserEmail("admin@admin.com"));
            adminUser.ChangeStatus(UserStatus.Active);

            var userCreatedResult = await _bonIdentityUserManager.CreateAsync(adminUser, "Admin@123456");

            if (userCreatedResult.IsSuccess)
            {
                _logger.LogInformation("Admin user created successfully with ID: {AdminUserId}", adminUser.Id);

                // Assign Admin Role to Admin User
                var assignRoleResult = await _bonIdentityUserManager.AssignRolesAsync(userCreatedResult.Value, new[] { adminRoleId });

                if (assignRoleResult.IsSuccess)
                    _logger.LogInformation("Admin role assigned successfully to admin user.");
                else
                    _logger.LogError("Failed to assign admin role to admin user. Errors: {Errors}",
                        assignRoleResult.ErrorMessage);
            }
            else
            {
                _logger.LogError("Failed to create admin user. Errors: {Errors}", userCreatedResult.ErrorMessage);
            }

            await uow.CompleteAsync(stoppingToken);
            _logger.LogInformation("Data seeding process completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during the data seeding process.");
        }
    }
}