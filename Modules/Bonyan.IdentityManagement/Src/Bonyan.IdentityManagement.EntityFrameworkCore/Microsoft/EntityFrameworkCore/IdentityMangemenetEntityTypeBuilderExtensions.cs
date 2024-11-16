using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Microsoft.EntityFrameworkCore;

public static class BonIdentityManagementEntityTypeBuilderExtensions
{
    public static ModelBuilder ConfigureBonIdentityManagementByConvention<TUser>(this ModelBuilder modelBuilder)
        where TUser : class, IBonIdentityUser
    {
        // Configure User Management base conventions
        modelBuilder.ConfigureBonUserManagementByConvention<TUser>();

        // Configure BonIdentityRole
        modelBuilder.Entity<BonIdentityRole>(builder =>
        {
            builder.ConfigureByConvention();
            builder.ToTable("Roles"); // Name the table
            builder.HasMany(role => role.Permissions)
                .WithMany()
                .UsingEntity("RolePermissions"); // Join table for Role-Permission
        });

        // Configure BonIdentityPermission
        modelBuilder.Entity<BonIdentityPermission>(builder =>
        {
            builder.ConfigureByConvention();
            builder.ToTable("Permissions"); // Name the table
            builder.HasKey(permission => permission.Key);
        });

        // Configure BonIdentityUserRoles (Join Table)
        modelBuilder.Entity<BonIdentityUserRoles>(builder =>
        {
            builder.ToTable("UserRoles"); // Name the join table
            builder.HasOne<TUser>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne<BonIdentityRole>()
                .WithMany()
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasKey(x => new {x.RoleId,x.UserId});
            ;
        });

        return modelBuilder;
    }

    public static ModelBuilder ConfigureBonIdentityManagementByConvention(this ModelBuilder modelBuilder)
    {
        // Configure BonIdentityManagement using BonIdentityUser as the default user type
        modelBuilder.ConfigureBonIdentityManagementByConvention<BonIdentityUser>();
        return modelBuilder;
    }
}
