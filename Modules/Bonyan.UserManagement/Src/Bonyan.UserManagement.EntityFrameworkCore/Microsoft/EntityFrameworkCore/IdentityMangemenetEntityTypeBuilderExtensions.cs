
using Bonyan.Layer.Domain.Enumerations;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.Enumerations;

namespace Microsoft.EntityFrameworkCore;

public static class BonIdentityManagementEntityTypeBuilderExtensions
{
    public static ModelBuilder ConfigureBonUserManagementByConvention<TUser>(this ModelBuilder modelBuilder) where TUser: BonUser
    {
        modelBuilder.Entity<TUser>().ConfigureByConvention();
        modelBuilder.Entity<TUser>().OwnsOne(c => c.Email, v =>
        {
            v.Property(x => x.Address).HasColumnName("EmailAddress");
            v.Property(x => x.IsVerified).HasColumnName("EmailIsVerified");
        });
        
        modelBuilder.Entity<TUser>().OwnsOne(c => c.PhoneNumber, v =>
        {
            v.Property(x => x.Number).HasColumnName("PhoneNumber");
            v.Property(x => x.IsVerified).HasColumnName("PhoneNumberIsVerified");
        });
        modelBuilder.Entity<TUser>().OwnsOne(c => c.Password, v =>
        {
            v.Property(x => x.HashedPassword).HasColumnName("PasswordHash");
            v.Property(x => x.Salt).HasColumnName("PasswordSalt");
        });
        modelBuilder.Entity<TUser>().Property(x => x.Status).HasConversion(x => x.Name,
            v => BonEnumeration.FromName<UserStatus>(v) ?? UserStatus.PendingApproval);
        modelBuilder.Entity<TUser>().HasIndex(x => x.UserName).IsUnique();
        modelBuilder.Entity<TUser>()
            .Property(p => p.Version)
            .IsConcurrencyToken();
        return modelBuilder;
    }
}