using Bonyan.EntityFrameworkCore;
using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.EntityFrameworkCore;
using Bonyan.TenantManagement.Domain;
using Bonyan.TenantManagement.EntityFrameworkCore;
using BonyanTemplate.Domain.Authors;
using BonyanTemplate.Domain.Books;
using BonyanTemplate.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace BonyanTemplate.Infrastructure.Data;

public class TemplateBookManagementBonDbContext : BonDbContext<TemplateBookManagementBonDbContext>,
    IBonTenantDbContext, IBonIdentityManagementDbContext<User>
{
    public TemplateBookManagementBonDbContext(DbContextOptions<TemplateBookManagementBonDbContext> options) :
        base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Book>().ConfigureByConvention();
        modelBuilder.Entity<Book>().HasOne(x => x.Author).WithMany().HasForeignKey(x => x.AuthorId);
        modelBuilder.Entity<Authors>().ConfigureByConvention();
        modelBuilder.ConfigureTenantManagementByConvention();
        modelBuilder.ConfigureIdentityManagementModelBuilder<User>();
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Authors> Authors { get; set; }
    public DbSet<BonTenant> Tenants { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<BonIdentityRole> Roles { get; set; }
    public DbSet<BonIdentityPermission> Permissions { get; set; }
}