using System.Text.RegularExpressions;

namespace Bonyan.IdentityManagement.Application.Roles;

/// <summary>
/// Normalizes role key for use as role Id: lowercase, spaces to '-', collapse multiple dashes, trim.
/// </summary>
public static class RoleKeyNormalizer
{
    private static readonly Regex MultipleDashes = new Regex(@"-+", RegexOptions.Compiled);

    /// <summary>
    /// Normalizes the role key: trim, to lower invariant, replace spaces with '-', collapse consecutive dashes, trim dashes from edges.
    /// </summary>
    /// <example>"Content Manager" -> "content-manager"; "  Admin  " -> "admin".</example>
    public static string Normalize(string? key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return string.Empty;
        var s = key.Trim().ToLowerInvariant();
        s = s.Replace(' ', '-');
        s = MultipleDashes.Replace(s, "-");
        return s.Trim('-');
    }
}
