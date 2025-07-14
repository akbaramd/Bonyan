using Bonyan.Localization.Attributes;

namespace Bonyan.Localization.Examples
{
    /// <summary>
    /// Example resource class demonstrating localization attributes
    /// </summary>
    [LocalizationResourceName("Test")]
    [InheritResource(typeof(AbpValidationResource))]
    public class TestResource
    {
        // This is a marker class for localization resource
        // The actual localization strings will be loaded from JSON files
    }

    /// <summary>
    /// Example base resource for validation messages
    /// </summary>
    [LocalizationResourceName("AbpValidation")]
    public class AbpValidationResource
    {
        // Base resource for validation messages
    }
} 