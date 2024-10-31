using System.Text.RegularExpressions;
using Bonyan.Layer.Domain.ValueObjects;

namespace Bonyan.UserManagement.Domain.ValueObjects
{
    /// <summary>
    /// Represents an email address value object with validation and verification status.
    /// </summary>
    public class Email : ValueObject
    {
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public string Address { get; }
        public bool IsVerified { get; private set; }

        public Email(string address)
        {
            if (string.IsNullOrWhiteSpace(address) || !EmailRegex.IsMatch(address))
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

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Address;
            yield return IsVerified;
        }

        public override string ToString() => Address;
    }
}