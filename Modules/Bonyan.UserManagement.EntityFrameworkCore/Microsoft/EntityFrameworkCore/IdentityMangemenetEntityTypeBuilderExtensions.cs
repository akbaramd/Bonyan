
using Bonyan.UserManagement.Domain;

namespace Microsoft.EntityFrameworkCore;

public static class IdentityManagementEntityTypeBuilderExtensions
{
    public static ModelBuilder ConfigureUserManagementByConvention<TUser>(this ModelBuilder modelBuilder) where TUser: BonyanUser
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
        modelBuilder.Entity<TUser>().HasIndex(x => x.UserName).IsUnique();
    
        return modelBuilder;
    }
}