using System;

namespace Bonyan.Localization.Attributes
{
    /// <summary>
    /// Attribute to specify base resource types that this resource inherits from.
    /// This allows resource inheritance and extension.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class InheritResourceAttribute : Attribute
    {
        public Type[] BaseResourceTypes { get; }

        public InheritResourceAttribute(params Type[] baseResourceTypes)
        {
            BaseResourceTypes = baseResourceTypes ?? throw new ArgumentNullException(nameof(baseResourceTypes));
        }

        public InheritResourceAttribute(Type baseResourceType)
        {
            if (baseResourceType == null)
                throw new ArgumentNullException(nameof(baseResourceType));
            
            BaseResourceTypes = new[] { baseResourceType };
        }
    }
} 