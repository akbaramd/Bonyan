using System.Security.Claims;

namespace Bonyan.User;

public interface ICurrentUser
{
  bool IsAuthenticated { get; }

  Guid? Id { get; }

  string? UserName { get; }

  string? Name { get; }

  string? SurName { get; }

  string? PhoneNumber { get; }

  bool PhoneNumberVerified { get; }

  string? Email { get; }

  bool EmailVerified { get; }

  Guid? TenantId { get; }

  string[] Roles { get; }

  Claim? FindClaim(string claimType);

  Claim[] FindClaims(string claimType);

  Claim[] GetAllClaims();

  bool IsInRole(string roleName);
}
