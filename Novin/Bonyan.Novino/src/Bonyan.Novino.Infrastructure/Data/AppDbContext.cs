using Bonyan.EntityFrameworkCore;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.EntityFrameworkCore;
using Bonyan.Novino.Domain.Entities;
using Bonyan.TenantManagement.Domain;
using Bonyan.TenantManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.Novino.Infrastructure.Data;

public class AppDbContext : BonDbContext<AppDbContext>
    ,IBonIdentityManagementDbContext<Domain.Entities.User, Role>
,IBonTenantDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        modelBuilder.ConfigureIdentityManagement<Domain.Entities.User,Role>();
    }

    public DbSet<Domain.Entities.User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<BonIdentityUserToken<Domain.Entities.User, Role>> UserTokens { get; set; }
    public DbSet<BonIdentityUserRoles<Domain.Entities.User, Role>> UserRoles { get; set; }
    public DbSet<BonIdentityUserClaims<Domain.Entities.User, Role>> UserClaims { get; set; }
    public DbSet<BonIdentityRoleClaims<Role>> RoleClaims { get; set; }
    public DbSet<BonTenant> Tenants { get; set; }
}