using System;
using System.Globalization;
using System.ComponentModel;

namespace Bonyan.Layer.Domain.ValueObjects
{
    [TypeConverter(typeof(BonBusinessIdTypeConverter))]
    public abstract class BonBusinessId<T, TKey> : BonValueObject
        where T : BonBusinessId<T, TKey>, new()
    {
        public TKey Value { get; private set; }

        protected BonBusinessId()
        {
        }

        protected BonBusinessId(TKey value)
        {
            if (value == null || value.Equals(default(TKey)))
                throw new ArgumentException($"The value of {nameof(value)} cannot be null or default.", nameof(value));

            Value = value;
        }

        /// <summary>
        /// Factory method to create an instance of the derived class from an existing value.
        /// </summary>
        public static T FromValue(TKey value)
        {
            if (value == null || value.Equals(default(TKey)))
                throw new ArgumentException($"The value of {nameof(value)} cannot be null or default.", nameof(value));

            return NewId(value);
        }

        /// <summary>
        /// Helper method to create an instance of the derived class.
        /// </summary>
        public static T NewId(TKey value)
        {
            var instance = new T();
            instance.Value = value;
            return instance;
        }

        // Overrides ToString for easier debugging and display purposes
        public override string ToString()
        {
            return Value?.ToString() ?? string.Empty;
        }

        // Overrides equality components for comparing value objects
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

     
    }
}
namespace Bonyan.Layer.Domain.ValueObjects
{
    /// <summary>
    /// Represents a specialized implementation of BonBusinessId with a GUID as the key type.
    /// </summary>
    [TypeConverter(typeof(BonBusinessIdTypeConverter))]
    public abstract class BonBusinessId<T> : BonBusinessId<T, Guid> where T : BonBusinessId<T>, new()
    {
        public BonBusinessId() : base(Guid.NewGuid())
        {
            // Default constructor prevents direct instantiation without valid GUID
        }

        public BonBusinessId(Guid value) : base(value)
        {
        }

        /// <summary>
        /// Factory method to create a new BusinessId with a new GUID.
        /// </summary>
        public static T NewId()
        {
            return FromValue(Guid.NewGuid());
        }

        /// <summary>
        /// Factory method to create a BusinessId from a string representation of a GUID.
        /// </summary>
     

        public static T FromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Business ID cannot be empty or whitespace.", nameof(value));

            if (!Guid.TryParse(value, out var parsedGuid))
                throw new ArgumentException("Invalid GUID format.", nameof(value));

            return FromValue(parsedGuid);
        }
    }
}