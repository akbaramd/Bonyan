using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.EntityFrameworkCore
{
    public static class BonIdentityManagementEntityTypeBuilderExtensions
    {
        public static ModelBuilder ConfigureIdentityManagement<TUser>(this ModelBuilder modelBuilder)
            where TUser : class, IBonIdentityUser
        {
            // Configure User Management base conventions
            modelBuilder.ConfigureUserManagement<TUser>();

            // Configure BonIdentityUser
            modelBuilder.Entity<TUser>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ConfigurePassword();

                builder.HasMany(x => x.UserRoles)
                    .WithOne()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure BonIdentityUserToken
            modelBuilder.Entity<BonIdentityUserToken>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ToTable("UserTokens");
                builder.HasOne<TUser>()
                    .WithMany(x => x.Tokens)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade); // Tokens are deleted when the user is deleted
            });

            // Configure BonIdentityRole
            modelBuilder.Entity<BonIdentityRole>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ToTable("Roles");
                
            });

            // Configure BonIdentityPermission
            modelBuilder.Entity<BonIdentityPermission>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ToTable("Permissions"); // Use a distinct table name for permissions
            });

            // Configure BonIdentityUserRoles (Join Table)
            modelBuilder.Entity<BonIdentityUserRoles>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ToTable("UserRoles");
                builder
                    .HasKey(r => new { r.UserId, r.RoleId });
                builder.HasOne<TUser>()
                    .WithMany(x => x.UserRoles)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade); // Deletes UserRoles when a user is deleted

                builder.HasOne(x => x.Role)
                    .WithMany()
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade); // Deletes UserRoles when a role is deleted

                builder.HasKey(x => new { x.RoleId, x.UserId });
            });

            // Configure BonIdentityRolePermissions (Join Table)
            modelBuilder.Entity<BonIdentityRolePermissions>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ToTable("RolePermissions"); // Ensure this table is distinct
                    builder.HasKey(r => new { r.PermissionId, r.RoleId });;
                builder.HasOne(x => x.Role)
                    .WithMany(x => x.RolePermissions)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade); // Deletes RolePermissions when a role is deleted

                builder.HasOne(x => x.Permission)
                    .WithMany(x=>x.RolePermissions)
                    .HasForeignKey(e => e.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade); // Deletes RolePermissions when a permission is deleted

                builder.HasKey(x => new { x.RoleId, x.PermissionId });
            });

            return modelBuilder;
        }

        /// <summary>
        /// Configures the password value object.
        /// </summary>
        private static void ConfigurePassword<TUser>(this EntityTypeBuilder<TUser> entity)
            where TUser : class, IBonIdentityUser
        {
            entity.OwnsOne(user => user.Password, password =>
            {
                password.Property(p => p.HashedPassword).HasColumnName("PasswordHash");
                password.Property(p => p.Salt).HasColumnName("PasswordSalt");
            });
        }

        public static ModelBuilder ConfigureIdentityManagement(this ModelBuilder modelBuilder)
        {
            // Configure BonIdentityManagement using BonIdentityUser as the default user type
            modelBuilder.ConfigureIdentityManagement<BonIdentityUser>();
            return modelBuilder;
        }
    }
}
