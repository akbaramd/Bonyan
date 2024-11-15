using Bonyan.Layer.Application.Abstractions;
using Bonyan.Layer.Application.Services;

namespace Bonyan.IdentityManagement.Application;

public interface IBonAuthService : IBonApplicationService
{
    Task<bool> LoginWithCookieAsync(string username, string password,bool isPersistent);
}