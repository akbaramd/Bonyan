using System.Reflection;
using AutoMapper.Internal;
using Bonyan.Layer.Domain.Audit.Abstractions;
using Bonyan.Layer.Domain.Entity;
using Bonyan.Layer.Domain.Enumerations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Bonyan.Layer.Domain.ValueObjects;
using Bonyan.MultiTenant;
using Microsoft.EntityFrameworkCore.Migrations.Internal;

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
            b.ConfigureEnumerationProperties();
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
        public static EntityTypeBuilder ConfigureEnumerationProperties(this EntityTypeBuilder builder)
        {
            var entityType = builder.Metadata.ClrType;
            Console.WriteLine(entityType.Name);
            // Find all properties of type BonEnumeration
            var enumerationProperties = entityType
                .GetProperties()
                .Where(property => IsEnumerationType(property.PropertyType))
                .ToList();

            
            foreach (var property in enumerationProperties)
            {
              
                var propertyType = property.PropertyType;
                var isNullable = IsNullableEnumerationType(propertyType);
                var underlyingType = isNullable ? Nullable.GetUnderlyingType(propertyType) : propertyType;

                if (underlyingType == null) continue;
                // Configure the enumeration with two separate columns for Id and Name
                builder.OwnsOne(propertyType,property.Name, c =>
                {
                    c.Property<int>("Id") // Map the `Id` property
                        .HasColumnName($"{property.Name}Id")
                        .IsRequired(!isNullable);

                    c.Property<string>("Name") // Map the `Name` property
                        .HasColumnName($"{property.Name}Name")
                        .IsRequired(!isNullable);
                });
            }

            return builder;
        }

        private static bool IsEnumerationType(Type type)
        {
            return type != null && type.IsSubclassOf(typeof(BonEnumeration));
        }
        public static EntityTypeBuilder ConfigureBusinessIdProperties(this EntityTypeBuilder builder)
        {
            try
            {
                var entityType = builder.Metadata.ClrType;

                // Find all properties of type BonBusinessId<T> or BonBusinessId<T, TKey>
                var businessIdProperties = entityType
                    .GetProperties()
                    .Where(property => IsBusinessIdType(property.PropertyType))
                    .ToList();

                foreach (var property in businessIdProperties)
                {
                    try
                    {
                        var propertyType = property.PropertyType;

                        // Check if the property is of type BonBusinessId<T, TKey> (including base types)
                        if (IsGenericBonBusinessIdWithKey(propertyType))
                        {

                            var keyType = GetKeyType(propertyType);

                            // Create a converter for BonBusinessId<T, TKey>
                            var converterType =
                                typeof(BonBusinessIdConverter<,>).MakeGenericType(propertyType, keyType);
                            if (Activator.CreateInstance(converterType) is ValueConverter converterInstance)
                            {
                                builder.Property(property.Name)
                                    .HasConversion(converterInstance)
                                    .HasColumnName(property.Name);
                            }
                        }
                        // Check if the property is of type BonBusinessId<T> (including base types)
                        else if (IsGenericBonBusinessId(propertyType))
                        {

                            // Create a converter for BonBusinessId<T>
                            var converterType = typeof(BonBusinessIdConverter<>).MakeGenericType(propertyType);
                            if (Activator.CreateInstance(converterType) is ValueConverter converterInstance)
                            {
                                builder.Property(property.Name)
                                    .HasConversion(converterInstance)
                                    .HasColumnName(property.Name);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log or handle exceptions for individual properties
                        Console.WriteLine($"Error configuring BusinessId property '{property.Name}': {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle any general exception
                Console.WriteLine($"Error configuring BusinessId properties: {ex.Message}");
            }

            return builder;
        }


        private static bool IsGenericBonBusinessIdWithKey(Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(BonBusinessId<,>))
                {
                    return true;
                }

                type = type.BaseType; // Traverse the base type hierarchy
            }

            return false;
        }


        private static bool IsGenericBonBusinessId(Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(BonBusinessId<>))
                {
                    return true;
                }

                type = type.BaseType; // Traverse the base type hierarchy
            }

            return false;
        }
        private static Type GetKeyType(Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(BonBusinessId<,>))
                {
                    return type.GetGenericArguments()[1]; // Return TKey
                }

                type = type.BaseType; // Traverse the base type hierarchy
            }

            throw new InvalidOperationException($"Type '{type.Name}' is not a valid BonBusinessId with a key.");
        }

        private static bool IsBusinessIdType(Type type)
        {
            try
            {
                // Check if the type is null
                if (type == null) return false;

                // Check if the type directly matches BonBusinessId<T> or BonBusinessId<T, TKey>
                if (IsGenericBonBusinessId(type) || IsGenericBonBusinessIdWithKey(type))
                    return true;

                // Traverse the inheritance hierarchy to check for base types
                var baseType = type.BaseType;
                while (baseType != null)
                {
                    if (IsGenericBonBusinessId(baseType) || IsGenericBonBusinessIdWithKey(baseType))
                        return true;

                    baseType = baseType.BaseType;
                }
            }
            catch (Exception ex)
            {
                // Log or handle exceptions during type checking
                Console.WriteLine($"Error checking if type '{type.Name}' is a BusinessId: {ex.Message}");
            }

            return false;
        }
        
        private static bool IsNullableEnumerationType(Type type)
        {
            return type.IsGenericType &&
                   type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                   type.GetGenericArguments()[0].IsSubclassOf(typeof(BonEnumeration));
        }
    }
}