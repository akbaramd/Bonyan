using Bonyan.Collections;
using System.Collections.Generic;

namespace Bonyan.Localization
{
    public class BonLocalizationOptions
    {
        public Dictionary<Type, LocalizationResourceInfo> ResourcesDictionary { get; set; }
        public Type DefaultResourceType { get; set; }
        public List<LanguageInfo> Languages { get; set; }
        public List<Action<object>> Configurators { get; }

        public BonLocalizationOptions()
        {
            ResourcesDictionary = new Dictionary<Type, LocalizationResourceInfo>();
            Languages = new List<LanguageInfo>();
            Configurators = new List<Action<object>>();
        }

        /// <summary>
        /// Gets the resource builder for fluent configuration
        /// </summary>
        public BonLocalizationResourceBuilder Resources => new BonLocalizationResourceBuilder(this);

        public void AddConfigurator(Action<object> configurator)
        {
            Configurators.Add(configurator);
        }

        public void SetDefaultResourceType<TResource>()
        {
            DefaultResourceType = typeof(TResource);
        }

        public void AddLanguage(string cultureCode, string uiCultureCode = null, string displayName = null)
        {
            Languages.Add(new LanguageInfo(cultureCode, uiCultureCode ?? cultureCode, displayName ?? cultureCode));
        }

        public LocalizationResourceInfo GetResource<TResource>()
        {
            return GetResource(typeof(TResource));
        }

        public LocalizationResourceInfo GetResource(Type resourceType)
        {
            if (ResourcesDictionary.TryGetValue(resourceType, out var resourceInfo))
            {
                return resourceInfo;
            }

            resourceInfo = new LocalizationResourceInfo(resourceType);
            ResourcesDictionary[resourceType] = resourceInfo;
            return resourceInfo;
        }

        public LocalizationResourceInfo AddResource<TResource>(string defaultCulture = "en")
        {
            return AddResource(typeof(TResource), defaultCulture);
        }

        public LocalizationResourceInfo AddResource(Type resourceType, string defaultCulture = "en")
        {
            var resourceInfo = new LocalizationResourceInfo(resourceType, defaultCulture);
            ResourcesDictionary[resourceType] = resourceInfo;
            return resourceInfo;
        }
    }

    public class LocalizationResourceInfo
    {
        public Type ResourceType { get; }
        public string DefaultCulture { get; set; }
        public string ResourceName { get; set; }
        public List<Type> BaseResourceTypes { get; set; }
        public List<string> VirtualJsonPaths { get; set; }
        public Dictionary<string, object> Properties { get; set; }

        public LocalizationResourceInfo(Type resourceType, string defaultCulture = "en")
        {
            ResourceType = resourceType;
            DefaultCulture = defaultCulture;
            BaseResourceTypes = new List<Type>();
            VirtualJsonPaths = new List<string>();
            Properties = new Dictionary<string, object>();
        }

        public LocalizationResourceInfo AddVirtualJson(string virtualPath)
        {
            VirtualJsonPaths.Add(virtualPath);
            return this;
        }

        public LocalizationResourceInfo AddBaseTypes(params Type[] baseResourceTypes)
        {
            BaseResourceTypes.AddRange(baseResourceTypes);
            return this;
        }

        public LocalizationResourceInfo SetResourceName(string resourceName)
        {
            ResourceName = resourceName;
            return this;
        }
    }

    public class LanguageInfo
    {
        public string CultureName { get; set; }
        public string UiCultureName { get; set; }
        public string DisplayName { get; set; }
        public string FlagIcon { get; set; }

        public LanguageInfo(string cultureName, string uiCultureName = null, string displayName = null)
        {
            CultureName = cultureName;
            UiCultureName = uiCultureName ?? cultureName;
            DisplayName = displayName ?? cultureName;
        }
    }
} 