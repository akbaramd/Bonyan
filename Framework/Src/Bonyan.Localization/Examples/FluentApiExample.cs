using Bonyan.Localization.Examples;

namespace Bonyan.Localization.Examples
{
    /// <summary>
    /// Example demonstrating the fluent API for configuring localization resources
    /// </summary>
    public static class FluentApiExample
    {
        public static void ConfigureLocalization(BonLocalizationOptions options)
        {
            // Example 1: Add a resource with default culture and virtual JSON path
            options.Resources
                .Add<TestResource>("en") // Define the resource by "en" default culture
                .AddVirtualJson("/Localization/Resources/Test") // Add strings from virtual json files
                .AddBaseTypes(typeof(AbpValidationResource)); // Inherit from an existing resource

            // Example 2: Get an existing resource and add more configuration
            options.Resources
                .Get<TestResource>()
                .AddVirtualJson("/Localization/Resources/Test/Extensions");

            // Example 3: Add another resource
            options.Resources
                .Add<AbpValidationResource>("en")
                .AddVirtualJson("/Localization/Resources/AbpValidation")
                .SetResourceName("AbpValidation");

            // Example 4: Configure languages
            options.AddLanguage("en", "en", "English");
            options.AddLanguage("fa", "fa", "فارسی");
            options.AddLanguage("ar", "ar", "العربية");

            // Example 5: Set default resource type
            options.SetDefaultResourceType<TestResource>();
        }
    }
} 