using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.DomainServices;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.UnitOfWork;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using BonyanTemplate.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BonyanTemplate.Infrastructure.Data.Identity;

/// <summary>
/// Seeds default Administrator role and admin user for the Bonyan Template.
/// Runs once at application startup.
/// </summary>
public class BonyanTemplateIdentityDataSeeder : IHostedService
{
    public const string AdministratorRoleId = "administrator";
    public const string AdministratorRoleTitle = "Administrator";
    public const string AdminUserName = "admin";
    public const string AdminDefaultPassword = "Admin@123";

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BonyanTemplateIdentityDataSeeder> _logger;

    public BonyanTemplateIdentityDataSeeder(
        IServiceProvider serviceProvider,
        ILogger<BonyanTemplateIdentityDataSeeder> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var migrationsSucceeded = false;
        using (var scope = _serviceProvider.CreateScope())
        {
            try
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BonyanTemplateDbContext>();
                await dbContext.Database.MigrateAsync(cancellationToken);
                migrationsSucceeded = true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Migrations could not be applied. Identity seed will be skipped. Ensure migrations exist and run: dotnet ef database update (or Update-Database from Package Manager Console).");
            }
        }

        if (!migrationsSucceeded)
        {
            _logger.LogInformation("Skipping identity seed because migrations were not applied.");
            return;
        }

        using (var scope2 = _serviceProvider.CreateScope())
        {
            var roleManager = scope2.ServiceProvider.GetService<IBonIdentityRoleManager>();
            var userManager = scope2.ServiceProvider.GetService<IBonIdentityUserManager>();
            var unitOfWork = scope2.ServiceProvider.GetService<IBonUnitOfWorkManager>();

            if (roleManager == null || userManager == null)
            {
                _logger.LogDebug("Identity managers not available; skipping identity seed.");
                return;
            }

            using var uow = unitOfWork.Begin();

            try
            {
                var roleId = BonRoleId.FromValue(AdministratorRoleId);

                var existingRole = await roleManager.FindRoleByIdAsync(AdministratorRoleId);
                if (!existingRole.IsSuccess || existingRole.Value == null)
                {
                    var role = new BonIdentityRole(roleId, AdministratorRoleTitle, canBeDeleted: false);
                    var createRoleResult = await roleManager.CreateRoleAsync(role);
                    if (createRoleResult.IsSuccess)
                        _logger.LogInformation("Seeded role: {RoleId} ({Title})", AdministratorRoleId, AdministratorRoleTitle);
                    else
                        _logger.LogWarning("Could not seed role {RoleId}: {Error}", AdministratorRoleId, createRoleResult.ErrorMessage);
                }

                var existingUser = await userManager.FindByUserNameAsync(AdminUserName);
                if (!existingUser.IsSuccess || existingUser.Value == null)
                {
                    var profile = new UserProfile("Admin", "User", displayName: "Administrator");
                    var user = new BonIdentityUser(BonUserId.NewId(), AdminUserName, profile);
                    user.SetGender(Gender.Male);
                    var createUserResult = await userManager.CreateAsync(user, AdminDefaultPassword);
                    if (!createUserResult.IsSuccess)
                    {
                        _logger.LogWarning("Could not seed admin user: {Error}", createUserResult.ErrorMessage);
                        return;
                    }
                    user = createUserResult.Value!;
                    var assignResult = await userManager.AssignRolesAsync(user, new[] { roleId });
                    if (assignResult.IsSuccess)
                        _logger.LogInformation("Seeded admin user: {UserName} (default password: {Password})", AdminUserName, AdminDefaultPassword);
                    else
                        _logger.LogWarning("Admin user created but could not assign Administrator role: {Error}", assignResult.ErrorMessage);
                }

                await uow.CompleteAsync();
            }
            catch (SqlException ex) when (ex.Number == 208 || ex.Message.Contains("Invalid object name", StringComparison.OrdinalIgnoreCase))
            {
                await uow.RollbackAsync();
                _logger.LogWarning(ex, "Identity tables (e.g. Roles) are missing. Run migrations from the project that contains BonyanTemplateDbContext: dotnet ef database update --project BonyanTemplate.Infrastructure --startup-project <your-host-project>. Then restart the application.");
                return;
            }
            catch (Exception e)
            {
                await uow.RollbackAsync();
                _logger.LogError(e, "Identity seed failed.");
                throw;
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
