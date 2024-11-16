using System.Security.Cryptography;
using Bonyan.Layer.Domain.ValueObjects;

namespace Bonyan.UserManagement.Domain.Users.ValueObjects
{
    /// <summary>
    /// Represents a password value object with hashing and verification.
    /// </summary>
    public class BonUserPassword : BonValueObject
    {
        private const int SaltSize = 16; // Size of salt in bytes
        private const int HashSize = 32; // Size of hash in bytes

        public string HashedPassword { get; }
        public byte[] Salt { get; }

        public BonUserPassword(string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword) || plainPassword.Length < 3)
            {
                throw new ArgumentException("Password must be at least 8 characters long.", nameof(plainPassword));
            }

            Salt = GenerateSalt();
            HashedPassword = HashPassword(plainPassword, Salt);
        }

        private BonUserPassword(string hashedPassword, byte[] salt)
        {
            HashedPassword = hashedPassword;
            Salt = salt;
        }

        /// <summary>
        /// Verifies if the provided plain password matches the stored hashed password.
        /// </summary>
        public bool Verify(string plainPassword)
        {
            var hashedInput = HashPassword(plainPassword, Salt);
            return HashedPassword == hashedInput;
        }

        private static byte[] GenerateSalt()
        {
            using var rng = new RNGCryptoServiceProvider();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);
            return salt;
        }

        private static string HashPassword(string password, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            return Convert.ToBase64String(pbkdf2.GetBytes(HashSize));
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return HashedPassword;
            yield return Salt;
        }

        public override string ToString() => "********";
    }
}
