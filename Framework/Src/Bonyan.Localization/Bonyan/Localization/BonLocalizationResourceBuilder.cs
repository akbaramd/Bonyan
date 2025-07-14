using System;
using System.Collections.Generic;
using Bonyan.Localization.Attributes;
using Bonyan.Localization.Helpers;

namespace Bonyan.Localization
{
    /// <summary>
    /// Fluent API builder for configuring localization resources
    /// </summary>
    public class BonLocalizationResourceBuilder
    {
        private readonly BonLocalizationOptions _options;

        public BonLocalizationResourceBuilder(BonLocalizationOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Adds a resource with the specified default culture
        /// </summary>
        /// <typeparam name="TResource">The resource type</typeparam>
        /// <param name="defaultCulture">Default culture code (e.g., "en")</param>
        /// <returns>Resource configuration builder</returns>
        public LocalizationResourceConfigurationBuilder<TResource> Add<TResource>(string defaultCulture = "en")
        {
            var resourceInfo = _options.AddResource<TResource>(defaultCulture);
            
            // Check for LocalizationResourceName attribute
            var resourceName = LocalizationAttributeHelper.GetResourceName(typeof(TResource));
            if (!string.IsNullOrEmpty(resourceName))
            {
                resourceInfo.SetResourceName(resourceName);
            }

            // Check for InheritResource attribute
            var baseResourceTypes = LocalizationAttributeHelper.GetBaseResourceTypes(typeof(TResource));
            if (baseResourceTypes.Length > 0)
            {
                resourceInfo.AddBaseTypes(baseResourceTypes);
            }

            return new LocalizationResourceConfigurationBuilder<TResource>(resourceInfo);
        }

        /// <summary>
        /// Gets an existing resource for further configuration
        /// </summary>
        /// <typeparam name="TResource">The resource type</typeparam>
        /// <returns>Resource configuration builder</returns>
        public LocalizationResourceConfigurationBuilder<TResource> Get<TResource>()
        {
            var resourceInfo = _options.GetResource<TResource>();
            return new LocalizationResourceConfigurationBuilder<TResource>(resourceInfo);
        }
    }

    /// <summary>
    /// Configuration builder for a specific resource type
    /// </summary>
    /// <typeparam name="TResource">The resource type</typeparam>
    public class LocalizationResourceConfigurationBuilder<TResource>
    {
        private readonly LocalizationResourceInfo _resourceInfo;

        public LocalizationResourceConfigurationBuilder(LocalizationResourceInfo resourceInfo)
        {
            _resourceInfo = resourceInfo ?? throw new ArgumentNullException(nameof(resourceInfo));
        }

        /// <summary>
        /// Adds virtual JSON paths for loading localization strings
        /// </summary>
        /// <param name="virtualPath">Virtual path to JSON files (e.g., "/Localization/Resources/Test")</param>
        /// <returns>This builder for method chaining</returns>
        public LocalizationResourceConfigurationBuilder<TResource> AddVirtualJson(string virtualPath)
        {
            _resourceInfo.AddVirtualJson(virtualPath);
            return this;
        }

        /// <summary>
        /// Adds base resource types for inheritance
        /// </summary>
        /// <param name="baseResourceTypes">Base resource types to inherit from</param>
        /// <returns>This builder for method chaining</returns>
        public LocalizationResourceConfigurationBuilder<TResource> AddBaseTypes(params Type[] baseResourceTypes)
        {
            _resourceInfo.AddBaseTypes(baseResourceTypes);
            return this;
        }

        /// <summary>
        /// Sets the resource name
        /// </summary>
        /// <param name="resourceName">The resource name</param>
        /// <returns>This builder for method chaining</returns>
        public LocalizationResourceConfigurationBuilder<TResource> SetResourceName(string resourceName)
        {
            _resourceInfo.SetResourceName(resourceName);
            return this;
        }
    }
} 