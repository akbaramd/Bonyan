  using System;
  using System.Collections.Generic;

  namespace Bonyan.DomainDrivenDesign.Domain.ValueObjects
  {
      public abstract class BusinessId<T> : ValueObject where T : BusinessId<T>, new()
      {
          // The underlying value of the business ID
          public Guid Value { get; private set; }
          protected BusinessId()
          {
            Value = Guid.NewGuid();
          }
          // Protected constructor to allow subclassing
          protected BusinessId(Guid value)
          {
              if (value == Guid.Empty)
                  throw new ArgumentException("Business ID cannot be an empty GUID.", nameof(value));

              Value = value;
          }

          // Static factory method to create a new BusinessId with a new GUID
          public static T CreateNew()
          {
              return Create(Guid.NewGuid());
          }

          // Static method to create a BusinessId from an existing GUID
          public static T FromGuid(Guid guid)
          {
              return Create(guid);
          }

          // Static method to create a BusinessId from a string representation of a GUID
          public static T FromString(string value)
          {
              if (string.IsNullOrWhiteSpace(value))
                  throw new ArgumentException("Business ID cannot be empty or whitespace.", nameof(value));

              if (!Guid.TryParse(value, out var parsedGuid))
                  throw new ArgumentException("Invalid GUID format.", nameof(value));

              return Create(parsedGuid);
          }

          // Helper method to create an instance of the derived class
          private static T Create(Guid value)
          {
              // Use reflection to create an instance of the derived class
              var instance = new T();
              instance.Initialize(value);
              return instance;
          }

          // Protected initialization method to set the value after creation
          protected void Initialize(Guid value)
          {
              // This is called only once during the creation process
              var obj = (T)(object)this;
              obj.Value = value;
          }

          // Overrides ToString for easier debugging and display purposes
          public override string ToString()
          {
              return Value.ToString();
          }

          // Overrides equality components for comparing value objects
          protected override IEnumerable<object?> GetEqualityComponents()
          {
              yield return Value;
          }
      }
  }
