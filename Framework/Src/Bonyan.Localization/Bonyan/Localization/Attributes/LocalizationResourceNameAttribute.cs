using System;

namespace Bonyan.Localization.Attributes
{
    /// <summary>
    /// Attribute to specify a short name for localization resources.
    /// This makes it easier to use localization texts on the client side.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class LocalizationResourceNameAttribute : Attribute
    {
        public string Name { get; }

        public LocalizationResourceNameAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
} 