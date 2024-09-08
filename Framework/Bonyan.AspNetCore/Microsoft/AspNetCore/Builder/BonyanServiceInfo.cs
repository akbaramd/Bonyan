namespace Microsoft.AspNetCore.Builder
{
  public class BonyanServiceInfo
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public string Version { get; set; }

    // Constructor with normalization logic
    public BonyanServiceInfo(string id, string name, string version)
    {
      Id = NormalizeId(id);
      Name = name;
      Version = NormalizeVersion(version);
    }

    // Normalize Id to replace spaces with "-" and only keep letters and hyphens
    private string NormalizeId(string id)
    {
      if (string.IsNullOrWhiteSpace(id))
        return string.Empty;

      // Replace spaces with "-"
      id = id.Replace(" ", "-");

      // Remove any characters that are not letters or hyphens, and trim
      id = System.Text.RegularExpressions.Regex.Replace(id, @"[^a-zA-Z\-]", "").Trim();

      // Convert to lowercase
      return id.ToLowerInvariant();
    }

    // Normalize Version to ensure it's in valid version format (e.g., 1.0.0)
    private string NormalizeVersion(string version)
    {
      if (string.IsNullOrWhiteSpace(version))
        return "1.0.0"; // Default version if empty

      // Use regex to extract valid version format (e.g., 1.0.0)
      var match = System.Text.RegularExpressions.Regex.Match(version, @"^\d+\.\d+\.\d+$");

      // If version is valid, return it, otherwise return default
      return match.Success ? version : "1.0.0";
    }
  }
}
