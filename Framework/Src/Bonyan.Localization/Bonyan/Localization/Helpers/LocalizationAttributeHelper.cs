using System;
using System.Reflection;
using Bonyan.Localization.Attributes;

namespace Bonyan.Localization.Helpers
{
    /// <summary>
    /// Helper class for working with localization attributes
    /// </summary>
    public static class LocalizationAttributeHelper
    {
        /// <summary>
        /// Gets the LocalizationResourceName attribute from a resource type
        /// </summary>
        /// <param name="resourceType">The resource type to check</param>
        /// <returns>The LocalizationResourceName attribute or null if not found</returns>
        public static LocalizationResourceNameAttribute? GetResourceNameAttribute(Type resourceType)
        {
            return resourceType.GetCustomAttribute<LocalizationResourceNameAttribute>();
        }

        /// <summary>
        /// Gets the InheritResource attribute from a resource type
        /// </summary>
        /// <param name="resourceType">The resource type to check</param>
        /// <returns>The InheritResource attribute or null if not found</returns>
        public static InheritResourceAttribute? GetInheritResourceAttribute(Type resourceType)
        {
            return resourceType.GetCustomAttribute<InheritResourceAttribute>();
        }

        /// <summary>
        /// Checks if a type has the LocalizationResourceName attribute
        /// </summary>
        /// <param name="resourceType">The resource type to check</param>
        /// <returns>True if the attribute is present, false otherwise</returns>
        public static bool HasResourceNameAttribute(Type resourceType)
        {
            return GetResourceNameAttribute(resourceType) != null;
        }

        /// <summary>
        /// Checks if a type has the InheritResource attribute
        /// </summary>
        /// <param name="resourceType">The resource type to check</param>
        /// <returns>True if the attribute is present, false otherwise</returns>
        public static bool HasInheritResourceAttribute(Type resourceType)
        {
            return GetInheritResourceAttribute(resourceType) != null;
        }

        /// <summary>
        /// Gets the resource name from the LocalizationResourceName attribute
        /// </summary>
        /// <param name="resourceType">The resource type to check</param>
        /// <returns>The resource name or null if not found</returns>
        public static string? GetResourceName(Type resourceType)
        {
            var attr = GetResourceNameAttribute(resourceType);
            return attr?.Name;
        }

        /// <summary>
        /// Gets the base resource types from the InheritResource attribute
        /// </summary>
        /// <param name="resourceType">The resource type to check</param>
        /// <returns>Array of base resource types or empty array if not found</returns>
        public static Type[] GetBaseResourceTypes(Type resourceType)
        {
            var attr = GetInheritResourceAttribute(resourceType);
            return attr?.BaseResourceTypes ?? Array.Empty<Type>();
        }
    }
} 