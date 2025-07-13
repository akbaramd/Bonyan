using System.Text.RegularExpressions;
using Bonyan.Layer.Domain.ValueObjects;

namespace Bonyan.UserManagement.Domain.Users.ValueObjects
{
    /// <summary>
    /// Represents an email address value object with validation and verification status.
    /// </summary>
    public class BonUserEmail : BonValueObject
    {
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public string Address { get; }
        public bool IsVerified { get; private set; }

        public BonUserEmail(string address)
        {
            if (!IsValidEmail(address))
            {
                throw new ArgumentException("Invalid email address.", nameof(address));
            }
            Address = address;
        }

        /// <summary>
        /// Verifies the email address.
        /// </summary>
        public void Verify()
        {
            IsVerified = true;
        }

        /// <summary>
        /// Static method to validate an email address.
        /// </summary>
        /// <param name="address">The email address to validate.</param>
        /// <returns>True if the email address is valid; otherwise, false.</returns>
        public static bool IsValidEmail(string address)
        {
            return !string.IsNullOrWhiteSpace(address) && EmailRegex.IsMatch(address);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Address;
            yield return IsVerified;
        }

        public override string ToString() => Address;
    }
}