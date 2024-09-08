namespace Bonyan;

public static class Check
{
  public static T NotNull<T>(T? value, string parameterName) where T : class
  {
    if (value is null)
    {
      throw new ArgumentNullException(parameterName, $"{parameterName} cannot be null.");
    }

    return value;
  }

  public static string NotNullOrEmpty(string value, string parameterName)
  {
    if (string.IsNullOrEmpty(value))
    {
      throw new ArgumentException($"{parameterName} cannot be null or empty.", parameterName);
    }

    return value;
  }

  public static IEnumerable<T> NotNullOrEmpty<T>(IEnumerable<T> collection, string parameterName)
  {
    if (collection is null || !collection.Any())
    {
      throw new ArgumentException($"{parameterName} cannot be null or empty.", parameterName);
    }

    return collection;
  }

  public static int InRange(int value, int minValue, int maxValue, string parameterName)
  {
    if (value < minValue || value > maxValue)
    {
      throw new ArgumentOutOfRangeException(parameterName,
        $"{parameterName} must be between {minValue} and {maxValue}.");
    }

    return value;
  }

  public static void EnsureTrue(bool condition, string parameterName, string message = "")
  {
    if (!condition)
    {
      throw new ArgumentException(string.IsNullOrWhiteSpace(message) ? $"{parameterName} condition failed." : message,
        parameterName);
    }
  }

  public static void EnsureFalse(bool condition, string parameterName, string message = "")
  {
    if (condition)
    {
      throw new ArgumentException(string.IsNullOrWhiteSpace(message) ? $"{parameterName} condition failed." : message,
        parameterName);
    }
  }
}
