using System.Text.RegularExpressions;
using Bonyan.Layer.Domain.ValueObjects;

namespace Bonyan.UserManagement.Domain.ValueObjects
{
    /// <summary>
    /// Represents a phone number value object with validation and verification status.
    /// </summary>
    public class PhoneNumber : ValueObject
    {
        private static readonly Regex PhoneRegex = new Regex(@"^\+?[1-9]\d{1,14}$", RegexOptions.Compiled);

        public string Number { get; }
        public bool IsVerified { get; private set; }

        public PhoneNumber(string number)
        {
            if (string.IsNullOrWhiteSpace(number) || !PhoneRegex.IsMatch(number))
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

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Number;
            yield return IsVerified;
        }

        public override string ToString() => Number;
    }
}