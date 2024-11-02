using Bonyan.EntityFrameworkCore;
using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.EntityFrameworkCore;
using Bonyan.TenantManagement.Domain;
using Bonyan.TenantManagement.EntityFrameworkCore;
using BonyanTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BonyanTemplate.Infrastructure.Data;

public class BonyanTemplateBookManagementDbContext : BonyanDbContext<BonyanTemplateBookManagementDbContext>,
    IBonyanTenantDbContext, IBonIdentityManagementDbContext<User, Role>
{
    public BonyanTemplateBookManagementDbContext(DbContextOptions<BonyanTemplateBookManagementDbContext> options) :
        base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Books>().ConfigureByConvention();
        modelBuilder.Entity<Books>().HasOne(x => x.Author).WithMany().HasForeignKey(x => x.AuthorId);
        modelBuilder.Entity<Authors>().ConfigureByConvention();
        modelBuilder.ConfigureTenantManagementByConvention();
        modelBuilder.ConfigureIdentityManagementByConvention<User, Role>();
    }

    public DbSet<Books> Books { get; set; }
    public DbSet<Authors> Authors { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<BonPermission> Permissions { get; set; }
    public DbSet<BonUserRole<User, Role>> UserRoles { get; set; }
}