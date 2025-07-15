using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.EntityFrameworkCore
{
    public static class BonIdentityManagementEntityTypeBuilderExtensions
    {
        public static ModelBuilder ConfigureIdentityManagement<TUser,TRole>(this ModelBuilder modelBuilder)
            where TUser : BonIdentityUser<TUser,TRole> where TRole : BonIdentityRole<TRole>
        {
            // Configure User Management base conventions
            modelBuilder.ConfigureUserManagement<TUser>(builder =>
            {
                builder.ConfigurePassword<TUser,TRole>();

                // Performance: Add indexes for commonly queried columns
                builder.HasIndex(x => x.UserName)
                    .HasDatabaseName("IX_Users_UserName")
                    .IsUnique();


                builder.HasIndex(x => x.CreatedAt)
                    .HasDatabaseName("IX_Users_CreatedAt");

                // Configure relationships (one-to-many)
                builder.HasMany(x => x.Tokens)
                    .WithOne()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasMany(x => x.UserRoles)
                    .WithOne()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasMany(x => x.UserClaims)
                    .WithOne()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Configure UserProfile with proper lengths
                builder.OwnsOne(user => user.Profile, profile =>
                {
                    profile.Property(p => p.FirstName)
                        .HasColumnName("FirstName")
                        .HasMaxLength(100)
                        .IsRequired();

                    profile.Property(p => p.LastName)
                        .HasColumnName("LastName")
                        .HasMaxLength(100)
                        .IsRequired();

                    profile.Property(p => p.DateOfBirth)
                        .HasColumnName("DateOfBirth")
                        .IsRequired(false);

                    profile.Property(p => p.NationalCode)
                        .HasColumnName("NationalCode")
                        .HasMaxLength(50)
                        .IsRequired(false);

                    // Performance: Index for searching by name
                    profile.HasIndex(p => new { p.FirstName, p.LastName })
                        .HasDatabaseName("IX_Users_FullName");
                });

                // Configure additional properties with proper lengths
                builder.Property(x => x.UserName)
                    .HasMaxLength(256)
                    .IsRequired();
            });

            // Configure BonIdentityUserToken
            modelBuilder.Entity<BonIdentityUserToken<TUser,TRole>>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ToTable("UserTokens");

                // Performance: Configure string lengths and indexes
                builder.Property(x => x.Type)
                    .HasMaxLength(100)
                    .IsRequired();

                builder.Property(x => x.Value)
                    .HasMaxLength(2000)
                    .IsRequired();

                // Performance: Composite index for token lookup
                builder.HasIndex(x => new { x.UserId, x.Type })
                    .HasDatabaseName("IX_UserTokens_UserId_Type")
                    .IsUnique();

                // Performance: Index for token cleanup by expiration
                builder.HasIndex(x => x.Expiration)
                    .HasDatabaseName("IX_UserTokens_Expiration")
                    .HasFilter("Expiration IS NOT NULL");

                // Performance: Index for created date
                builder.HasIndex(x => x.CreatedAt)
                    .HasDatabaseName("IX_UserTokens_CreatedAt");
            });

            // Configure BonIdentityRole
            modelBuilder.Entity<TRole>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ToTable("Roles");

                // Performance: Configure string lengths
                builder.Property(x => x.Title)
                    .HasMaxLength(256)
                    .IsRequired();

                // Performance: Unique index on title for role lookup
                builder.HasIndex(x => x.Title)
                    .HasDatabaseName("IX_Roles_Title")
                    .IsUnique();

                // Performance: Index for deletable roles
                builder.HasIndex(x => x.CanBeDeleted)
                    .HasDatabaseName("IX_Roles_CanBeDeleted");

                builder.HasMany(x => x.RoleClaims)
                    .WithOne(x => x.Role)
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });



            // Configure BonIdentityUserRoles (Join Table)
            modelBuilder.Entity<BonIdentityUserRoles<TUser,TRole>>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ToTable("UserRoles");

                // Performance: Composite primary key
                builder.HasKey(r => new { r.UserId, r.RoleId });

                // Performance: Index for role-based queries
                builder.HasIndex(x => x.RoleId)
                    .HasDatabaseName("IX_UserRoles_RoleId");

                // Performance: Index for user-based queries
                builder.HasIndex(x => x.UserId)
                    .HasDatabaseName("IX_UserRoles_UserId");

                builder.HasOne(x => x.Role)
                    .WithMany()
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure BonIdentityUserClaims
            modelBuilder.Entity<BonIdentityUserClaims<TUser,TRole>>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ToTable("UserClaims");

                // Performance: Configure string lengths
                builder.Property(x => x.ClaimType)
                    .HasMaxLength(256)
                    .IsRequired();

                builder.Property(x => x.ClaimValue)
                    .HasMaxLength(1000)
                    .IsRequired();

                builder.Property(x => x.Issuer)
                    .HasMaxLength(256);

                // Performance: Composite index for claim lookup
                builder.HasIndex(x => new { x.UserId, x.ClaimType })
                    .HasDatabaseName("IX_UserClaims_UserId_ClaimType");

                // Performance: Index for claim type searches
                builder.HasIndex(x => x.ClaimType)
                    .HasDatabaseName("IX_UserClaims_ClaimType");

                // Performance: Index for claim value searches
                builder.HasIndex(x => x.ClaimValue)
                    .HasDatabaseName("IX_UserClaims_ClaimValue");

                // Performance: Unique constraint for user-claim combination
                builder.HasIndex(x => new { x.UserId, x.ClaimType, x.ClaimValue })
                    .HasDatabaseName("IX_UserClaims_UserId_ClaimType_ClaimValue")
                    .IsUnique();
            });

            // Configure BonIdentityRoleClaims
            modelBuilder.Entity<BonIdentityRoleClaims<TRole>>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ToTable("RoleClaims");

                // Performance: Configure string lengths
                builder.Property(x => x.ClaimType)
                    .HasMaxLength(256)
                    .IsRequired();

                builder.Property(x => x.ClaimValue)
                    .HasMaxLength(1000)
                    .IsRequired();

                builder.Property(x => x.Issuer)
                    .HasMaxLength(256);

                // Performance: Composite index for claim lookup
                builder.HasIndex(x => new { x.RoleId, x.ClaimType })
                    .HasDatabaseName("IX_RoleClaims_RoleId_ClaimType");

                // Performance: Index for claim type searches
                builder.HasIndex(x => x.ClaimType)
                    .HasDatabaseName("IX_RoleClaims_ClaimType");

                // Performance: Index for claim value searches
                builder.HasIndex(x => x.ClaimValue)
                    .HasDatabaseName("IX_RoleClaims_ClaimValue");


                // Performance: Unique constraint for role-claim combination
                builder.HasIndex(x => new { x.RoleId, x.ClaimType, x.ClaimValue })
                    .HasDatabaseName("IX_RoleClaims_RoleId_ClaimType_ClaimValue")
                    .IsUnique();

                builder.HasOne(x => x.Role)
                    .WithMany(x => x.RoleClaims)
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            return modelBuilder;
        }

        /// <summary>
        /// Configures the password value object.
        /// </summary>
        private static void ConfigurePassword<TUser,TRole>(this EntityTypeBuilder<TUser> entity)
            where TUser : BonIdentityUser<TUser,TRole> where TRole : BonIdentityRole<TRole>
        {
            entity.OwnsOne(user => user.Password, password =>
            {
                password.Property(p => p.HashedPassword)
                    .HasColumnName("PasswordHash")
                    .HasMaxLength(256) // Standard base64 hash length
                    .IsRequired();

                password.Property(p => p.Salt)
                    .HasColumnName("PasswordSalt")
                    .IsRequired();
            });
        }

       
    }
}
