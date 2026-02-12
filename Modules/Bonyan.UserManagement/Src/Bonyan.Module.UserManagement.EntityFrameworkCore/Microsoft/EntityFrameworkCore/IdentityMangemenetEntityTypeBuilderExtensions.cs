using Bonyan.UserManagement.Domain.Users;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microsoft.EntityFrameworkCore;

public static class BonUserManagementEntityTypeBuilderExtensions
{
    /// <summary>
    /// پیکربندی نگاشت موجودیت کاربر (فقط دامنه کاربر؛ بدون احراز هویت).
    /// </summary>
    public static ModelBuilder ConfigureUserManagement<TUser>(this ModelBuilder modelBuilder, Action<EntityTypeBuilder<TUser>>? action = null)
        where TUser : class, IBonUser
    {
        var entity = modelBuilder.Entity<TUser>();

        entity.ToTable("Users");
        entity.ConfigureByConvention();

        ConfigureEmail(entity);
        ConfigurePhoneNumber(entity);
        ConfigureProfile(entity);
        ConfigurePreferredCulture(entity);
        ConfigureConcurrencyToken(entity);
        ConfigureIndexes(entity);

        entity.Property(nameof(IBonUser.TimeZoneId))
            .HasMaxLength(128)
            .IsRequired(false);
        entity.Property(nameof(IBonUser.AvatarUrl))
            .HasMaxLength(2048)
            .IsRequired(false);
        // Gender (BonEnumeration) is configured by ConfigureByConvention

        action?.Invoke(entity);
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

    private static void ConfigureProfile<TUser>(EntityTypeBuilder<TUser> entity)
        where TUser : class, IBonUser
    {
        entity.OwnsOne(user => user.Profile, profile =>
        {
            profile.Property(p => p.FirstName).HasColumnName("FirstName").HasMaxLength(100).IsRequired(false);
            profile.Property(p => p.LastName).HasColumnName("LastName").HasMaxLength(100).IsRequired(false);
            profile.Property(p => p.DisplayName).HasColumnName("DisplayName").HasMaxLength(256).IsRequired(false);
            profile.Property(p => p.DateOfBirth).HasColumnName("DateOfBirth").IsRequired(false);
            profile.Property(p => p.NationalCode).HasColumnName("NationalCode").HasMaxLength(50).IsRequired(false);
        });
    }

    private static void ConfigurePreferredCulture<TUser>(EntityTypeBuilder<TUser> entity)
        where TUser : class, IBonUser
    {
        entity.OwnsOne(user => user.PreferredCulture, culture =>
        {
            culture.Property(c => c.CultureName).HasColumnName("PreferredCulture").HasMaxLength(32).IsRequired(false);
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
