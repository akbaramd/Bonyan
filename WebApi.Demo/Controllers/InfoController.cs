using Bonyan.Modularity.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Demo.Modules;

namespace WebApi.Demo.Controllers;

/// <summary>
/// Application information API controller.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InfoController : ControllerBase
{
    private readonly WebApiDemoOptions _options;
    private readonly IWebBonModularityApplication _modularApp;

    public InfoController(
        IOptions<WebApiDemoOptions> options,
        IWebBonModularityApplication modularApp)
    {
        _options = options.Value;
        _modularApp = modularApp;
    }

    /// <summary>
    /// Gets application information.
    /// </summary>
    [HttpGet]
    public ActionResult<object> GetInfo()
    {
        return Ok(new
        {
            ApplicationName = _options.ApplicationName,
            Version = _options.Version,
            Modules = _modularApp.Modules.Select(m => new
            {
                Name = m.ModuleType.Name,
                Namespace = m.ModuleType.Namespace,
                Assembly = m.ModuleType.Assembly.GetName().Name,
                IsPlugin = m.IsPluginModule,
                Dependencies = m.Dependencies.Select(d => d.ModuleType.Name)
            })
        });
    }
}

