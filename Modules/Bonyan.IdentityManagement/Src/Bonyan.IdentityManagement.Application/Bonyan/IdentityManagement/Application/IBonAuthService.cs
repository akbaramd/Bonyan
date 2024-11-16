using Bonyan.Layer.Application.Abstractions;

namespace Bonyan.IdentityManagement.Application;

public interface IBonAuthService : IBonApplicationService
{
    Task<bool> LoginWithCookieAsync(string username, string password,bool isPersistent);
}