using System.Text.RegularExpressions;
using Bonyan.Layer.Domain.ValueObjects;

namespace Bonyan.UserManagement.Domain.ValueObjects
{
    /// <summary>
    /// Represents a phone number value object with validation and verification status.
    /// </summary>
    public class PhoneNumber : BonValueObject
    {
        private static readonly Regex PhoneRegex = new(@"^\+?[1-9]\d{1,14}$", RegexOptions.Compiled);

        public string Number { get; }
        public bool IsVerified { get; private set; }

        public PhoneNumber(string number)
        {
            // Trim the first '0' if it exists
            number = number.TrimStart('0');

            if (!IsValidPhoneNumber(number))
            {
                throw new ArgumentException("Invalid phone number.", nameof(number));
            }
            Number = number;
        }

        /// <summary>
        /// Verifies the phone number.
        /// </summary>
        public void Verify()
        {
            IsVerified = true;
        }

        /// <summary>
        /// Static method to validate a phone number.
        /// </summary>
        /// <param name="number">The phone number to validate.</param>
        /// <returns>True if the phone number is valid; otherwise, false.</returns>
        public static bool IsValidPhoneNumber(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
            {
                return false;
            }

            // Trim the first '0' for validation purposes
            number = number.TrimStart('0');
            return PhoneRegex.IsMatch(number);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Number;
            yield return IsVerified;
        }

        public override string ToString() => Number;
    }
}