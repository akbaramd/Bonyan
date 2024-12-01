using Bonyan.UserManagement.Domain.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microsoft.EntityFrameworkCore;

public static class BonUserManagementEntityTypeBuilderExtensions
{
    /// <summary>
    /// Configures the entity mappings for user management by convention.
    /// </summary>
    /// <typeparam name="TUser">The user entity type.</typeparam>
    /// <param name="modelBuilder">The model builder.</param>
    /// <returns>The configured model builder.</returns>
    public static ModelBuilder ConfigureUserManagementModelBuilder<TUser>(this ModelBuilder modelBuilder)
        where TUser : class, IBonUser
    {
        var entity = modelBuilder.Entity<TUser>();

        // Configure table name
        entity.ToTable("Users");

        // Apply default conventions
        entity.ConfigureByConvention();

        // Configure owned properties for value objects
        ConfigureEmail(entity);
        ConfigurePhoneNumber(entity);

        // Configure properties
        // ConfigureStatus(entity);
        ConfigureConcurrencyToken(entity);

        // Configure indexes
        ConfigureIndexes(entity);

        return modelBuilder;
    }

    /// <summary>
    /// Configures the email value object.
    /// </summary>
    private static void ConfigureEmail<TUser>(EntityTypeBuilder<TUser> entity)
        where TUser : class, IBonUser
    {
        entity.OwnsOne(user => user.Email, email =>
        {
            email.Property(e => e.Address).HasColumnName("EmailAddress");
            email.Property(e => e.IsVerified).HasColumnName("EmailIsVerified");
        });
    }

    /// <summary>
    /// Configures the phone number value object.
    /// </summary>
    private static void ConfigurePhoneNumber<TUser>(EntityTypeBuilder<TUser> entity)
        where TUser : class, IBonUser
    {
        entity.OwnsOne(user => user.PhoneNumber, phone =>
        {
            phone.Property(p => p.Number).HasColumnName("PhoneNumber");
            phone.Property(p => p.IsVerified).HasColumnName("PhoneNumberIsVerified");
        });
    }





    /// <summary>
    /// Configures the concurrency token.
    /// </summary>
    private static void ConfigureConcurrencyToken<TUser>(EntityTypeBuilder<TUser> entity)
        where TUser : class, IBonUser
    {
        entity.Property(user => user.Version)
            .IsConcurrencyToken();
    }

    /// <summary>
    /// Configures indexes for the entity.
    /// </summary>
    private static void ConfigureIndexes<TUser>(EntityTypeBuilder<TUser> entity)
        where TUser : class, IBonUser
    {
        entity.HasIndex(user => user.UserName)
            .IsUnique();
    }
}
