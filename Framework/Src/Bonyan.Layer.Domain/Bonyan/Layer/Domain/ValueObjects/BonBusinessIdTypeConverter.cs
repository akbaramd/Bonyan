using System.ComponentModel;
using System.Globalization;

namespace Bonyan.Layer.Domain.ValueObjects;

public class BonBusinessIdTypeConverter : TypeConverter
{
    // Determines if the conversion from string is supported for TKey
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        if (sourceType == typeof(string))
            return true;

        return base.CanConvertFrom(context, sourceType);
    }

    // Handles the conversion from string to BonBusinessId<T, TKey>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string str)
        {
            // Use reflection to check if the type has a Parse method
            var targetType = context?.Instance?.GetType();
            if (targetType == null)
            {
                return base.ConvertFrom(context, culture, value);
            }

            var parseMethod = targetType.GetMethod("Parse", new[] { typeof(string), typeof(IFormatProvider) });
            if (parseMethod != null)
            {
                try
                {
                    // If Parse method exists, use it to create an instance
                    return parseMethod.Invoke(null, new object[] { str, culture });
                }
                catch (Exception ex)
                {
                    throw new FormatException($"Failed to convert '{str}' to type '{targetType.Name}'.", ex);
                }
            }
            else
            {
                // Otherwise, handle some common types (like Guid, int, etc.)
                if (targetType == typeof(Guid))
                {
                    return Guid.TryParse(str, out var guid) ? guid : throw new FormatException($"Invalid GUID format: {str}");
                }
                else if (targetType == typeof(int))
                {
                    return int.TryParse(str, out var intValue) ? intValue : throw new FormatException($"Invalid integer format: {str}");
                }
                else if (targetType == typeof(string))
                {
                    return str;
                }
                else
                {
                    throw new NotSupportedException($"Conversion from string to {targetType.Name} is not supported.");
                }
            }
        }

        return base.ConvertFrom(context, culture, value);
    }
}