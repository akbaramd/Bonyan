using System.Net;
using Bonyan.TenantManagement.Domain;
using Exceptions;

namespace Bonyan.TenantManagement.Application.Exceptions;

/// <summary>
/// Custom exception thrown when a tenant cannot be found in the application layer.
/// </summary>
public class BonTenantNotFoundException : BonApplicationException
{
  /// <summary>
  /// Initializes a new instance of the <see cref="BonTenantNotFoundException"/> class.
  /// </summary>
  /// <param name="status">The HTTP status code to be returned, defaulting to NotFound.</param>
  /// <param name="tenantId">The ID of the tenant that was not found.</param>
  /// <param name="message">The error message, defaulting to a descriptive message.</param>
  /// <param name="errorCode">A specific error code, defaulting to "TenantNotFound".</param>
  /// <param name="parameters">Additional parameters for logging or debugging.</param>
  public BonTenantNotFoundException(
    BonTenantId? tenantId = null,
    string message = "The specified tenant could not be found in the application layer.",
    string errorCode = "TenantNotFound",
    object? parameters = null
  ) : base( HttpStatusCode.NotFound, $"{message} | Tenant ID: {tenantId?.Value.ToString()}", errorCode, parameters)
  {
    TenantId = tenantId;
  }

  /// <summary>
  /// Gets the ID of the tenant that was not found.
  /// </summary>
  public BonTenantId? TenantId { get; }
}
