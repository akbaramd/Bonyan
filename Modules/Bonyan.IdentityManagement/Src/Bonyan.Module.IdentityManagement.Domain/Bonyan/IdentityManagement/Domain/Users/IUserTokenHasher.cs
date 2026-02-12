namespace Bonyan.IdentityManagement.Domain.Users;

/// <summary>
/// Hashes token values so they are never stored or compared in plain form.
/// Implement in application/infrastructure and inject into <see cref="DomainServices.BonIdentityUserManager"/>.
/// </summary>
public interface IUserTokenHasher
{
    /// <summary>
    /// Returns a one-way hash of the token value. Same input must always produce the same hash.
    /// </summary>
    string Hash(string plainTokenValue);
}
