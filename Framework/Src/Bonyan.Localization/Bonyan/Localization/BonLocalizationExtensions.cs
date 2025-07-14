namespace Bonyan.Localization
{
    public static class BonLocalizationExtensions
    {
        public static BonLocalizationOptions AddConfigurator(this BonLocalizationOptions options, Action<object> configurator)
        {
            options.AddConfigurator(configurator);
            return options;
        }

        public static BonLocalizationOptions SetDefaultResourceType<TResource>(this BonLocalizationOptions options)
        {
            options.SetDefaultResourceType<TResource>();
            return options;
        }

        public static BonLocalizationOptions AddLanguage(this BonLocalizationOptions options, string cultureCode, string uiCultureCode = null, string displayName = null)
        {
            options.AddLanguage(cultureCode, uiCultureCode, displayName);
            return options;
        }

        public static LocalizationResourceInfo AddResource<TResource>(this BonLocalizationOptions options, string defaultCulture = "en")
        {
            return options.AddResource<TResource>(defaultCulture);
        }

        public static LocalizationResourceInfo AddResource(this BonLocalizationOptions options, Type resourceType, string defaultCulture = "en")
        {
            return options.AddResource(resourceType, defaultCulture);
        }

        public static LocalizationResourceInfo GetResource<TResource>(this BonLocalizationOptions options)
        {
            return options.GetResource<TResource>();
        }

        public static LocalizationResourceInfo GetResource(this BonLocalizationOptions options, Type resourceType)
        {
            return options.GetResource(resourceType);
        }

        public static LocalizationResourceInfo AddVirtualJson(this LocalizationResourceInfo resourceInfo, string virtualPath)
        {
            return resourceInfo.AddVirtualJson(virtualPath);
        }

        public static LocalizationResourceInfo AddBaseTypes(this LocalizationResourceInfo resourceInfo, params Type[] baseResourceTypes)
        {
            return resourceInfo.AddBaseTypes(baseResourceTypes);
        }

        public static LocalizationResourceInfo SetResourceName(this LocalizationResourceInfo resourceInfo, string resourceName)
        {
            return resourceInfo.SetResourceName(resourceName);
        }
    }
} 