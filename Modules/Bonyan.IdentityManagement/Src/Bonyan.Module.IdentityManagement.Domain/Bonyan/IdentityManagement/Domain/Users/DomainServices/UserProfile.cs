using Bonyan.Layer.Domain.ValueObjects;

namespace Bonyan.IdentityManagement.Domain.Users.DomainServices
{
    public class UserProfile : BonValueObject
    {
        public string FirstName { get; }
        public string LastName { get; }
        public DateTime? DateOfBirth { get; }
        public string NationalCode { get; }

        public UserProfile(string firstName, string lastName, DateTime? dateOfBirth, string nationalCode)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            DateOfBirth = dateOfBirth;
            NationalCode = nationalCode;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstName;
            yield return LastName;
            yield return DateOfBirth;
            yield return NationalCode;
        }
    }
} 