using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Bonyan.Layer.Domain.Abstractions;
using Bonyan.Layer.Domain.ValueObjects;
using Bonyan.MultiTenant;

namespace Microsoft.EntityFrameworkCore
{
    public static class EntityTypeBuilderExtensions
    {
        public static EntityTypeBuilder ConfigureByConvention(this EntityTypeBuilder b)
        {
            b.ConfigureCreationAuditable();
            b.ConfigureModificationAuditable();
            b.ConfigureSoftDeleteAuditable();
            b.ConfigureBusinessIdProperties();
            b.TryConfigureMultiTenant();
            return b;
        }
        public static void TryConfigureMultiTenant(this EntityTypeBuilder b)
        {
          if (b.Metadata.ClrType.IsAssignableTo(typeof(IBonMultiTenant)))
          {
            b.Property(nameof(IBonMultiTenant.TenantId))
              .IsRequired(false)
              .HasColumnName(nameof(IBonMultiTenant.TenantId));
          }
        }
        private static EntityTypeBuilder ConfigureCreationAuditable(this EntityTypeBuilder b)
        {
            try
            {
                if (b.Metadata.ClrType.IsAssignableTo(typeof(IBonCreationAuditable)))
                {
                    b.Property(nameof(IBonCreationAuditable.CreatedDate))
                        .IsRequired()
                        .HasColumnName(nameof(IBonCreationAuditable.CreatedDate));
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception if needed
                Console.WriteLine($"Error configuring creation auditable properties: {ex.Message}");
            }

            return b;
        }

        private static EntityTypeBuilder ConfigureModificationAuditable(this EntityTypeBuilder b)
        {
            try
            {
                if (b.Metadata.ClrType.IsAssignableTo(typeof(IBonModificationAuditable)))
                {
                    b.Property(nameof(IBonModificationAuditable.ModifiedDate))
                        .IsRequired(false)
                        .HasColumnName(nameof(IBonModificationAuditable.ModifiedDate));
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception if needed
                Console.WriteLine($"Error configuring modification auditable properties: {ex.Message}");
            }

            return b;
        }

        private static EntityTypeBuilder ConfigureSoftDeleteAuditable(this EntityTypeBuilder b)
        {
            try
            {
                if (b.Metadata.ClrType.IsAssignableTo(typeof(IBonSoftDeleteAuditable)))
                {
                    b.Property(nameof(IBonSoftDeleteAuditable.IsDeleted))
                        .IsRequired()
                        .HasDefaultValue(false)
                        .HasColumnName(nameof(IBonSoftDeleteAuditable.IsDeleted));

                    b.Property(nameof(IBonSoftDeleteAuditable.DeletedDate))
                        .IsRequired(false)
                        .HasColumnName(nameof(IBonSoftDeleteAuditable.DeletedDate));
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception if needed
                Console.WriteLine($"Error configuring soft delete auditable properties: {ex.Message}");
            }

            return b;
        }

        private static EntityTypeBuilder ConfigureBusinessIdProperties(this EntityTypeBuilder b)
        {
            try
            {
                var entityType = b.Metadata.ClrType;
                var businessIdProperties = entityType.GetProperties()
                    .Where(p => IsBusinessIdType(p.PropertyType))
                    .ToList();

                foreach (var property in businessIdProperties)
                {
                    try
                    {
                        var propertyType = property.PropertyType;
                        var converterType = typeof(BonBusinessIdConverter<>).MakeGenericType(propertyType);

                        // Try to create an instance of the converter
                        if (Activator.CreateInstance(converterType) is ValueConverter converterInstance)
                        {
                            b.Property(property.Name)
                                .HasConversion(converterInstance)
                                .HasColumnName(property.Name);
                        }
                        else
                        {
                            Console.WriteLine($"Failed to create converter for property {property.Name} of type {propertyType.Name}.");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log or handle the exception for each property configuration
                        Console.WriteLine($"Error configuring BusinessId property '{property.Name}': {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle any general exception that occurs in this method
                Console.WriteLine($"Error configuring BusinessId properties: {ex.Message}");
            }

            return b;
        }

        private static bool IsBusinessIdType(Type type)
        {
            try
            {
                // Check if the type is directly a generic BusinessId<>
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(BonBusinessId<>))
                {
                    return true;
                }

                // Check if the type inherits from a generic BusinessId<>
                var baseType = type.BaseType;
                while (baseType != null)
                {
                    if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(BonBusinessId<>))
                    {
                        return true;
                    }
                    baseType = baseType.BaseType;
                }
            }
            catch (Exception ex)
            {
                // Log or handle exceptions that occur during type checking
                Console.WriteLine($"Error checking if type '{type.Name}' is a BusinessId: {ex.Message}");
            }

            return false;
        }
    }
}
