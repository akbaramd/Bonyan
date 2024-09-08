namespace Bonyan.AspNetCore.Extensions
{
  public static class UtilityExtensions
  {
    // Normalize job type name (replace spaces with hyphens and lowercase)
    public static string NormalizeJobType(this string jobTypeName)
    {
      return jobTypeName.Replace(" ", "-").ToLowerInvariant();
    }

    // Add console messages to the list
    public static void AddConsoleMessage(this IServiceCollection services, string message, string category)
    {
      // Add logic to store console messages in your DI if needed
      Console.WriteLine($"[{category}] {message}");
    }
  }
}
