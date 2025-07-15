using Bonyan.EntityFrameworkCore;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.EntityFrameworkCore;
using Bonyan.Novino.Web.Models;
using Bonyan.TenantManagement.Domain;
using Bonyan.TenantManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.Novino.Web.Data;

public class AppDbContext : BonDbContext<AppDbContext>
    ,IBonIdentityManagementDbContext<Models.User, Models.Role>
,IBonTenantDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        modelBuilder.ConfigureIdentityManagement<Models.User,Models.Role>();
    }

    public DbSet<Models.User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<BonIdentityUserToken<Models.User, Role>> UserTokens { get; set; }
    public DbSet<BonIdentityUserRoles<Models.User, Role>> UserRoles { get; set; }
    public DbSet<BonIdentityUserClaims<Models.User, Role>> UserClaims { get; set; }
    public DbSet<BonIdentityRoleClaims<Role>> RoleClaims { get; set; }
    public DbSet<BonTenant> Tenants { get; set; }
}