using Microsoft.AspNetCore.Mvc;
using WebApi.Demo.Demos.Models;
using WebApi.Demo.Demos.Services;

namespace WebApi.Demo.Controllers;

/// <summary>
/// Demo API controller for testing attributed services ([BonScoped], [BonSingleton], [BonTransient]).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DemoController : ControllerBase
{
    private readonly IDemoService _demoService;
    private readonly IAppClockService _clockService;
    private readonly IIdGeneratorService _idGenerator;
    private readonly ILogger<DemoController> _logger;

    public DemoController(
        IDemoService demoService,
        IAppClockService clockService,
        IIdGeneratorService idGenerator,
        ILogger<DemoController> logger)
    {
        _demoService = demoService;
        _clockService = clockService;
        _idGenerator = idGenerator;
        _logger = logger;
    }

    /// <summary>
    /// Gets all demo items (from [BonScoped] DemoService).
    /// </summary>
    [HttpGet("items")]
    public async Task<ActionResult<IReadOnlyList<DemoItem>>> GetItems()
    {
        var items = await _demoService.GetItemsAsync();
        return Ok(items);
    }

    /// <summary>
    /// Gets a demo item by ID.
    /// </summary>
    [HttpGet("items/{id}")]
    public async Task<ActionResult<DemoItem>> GetItem(int id)
    {
        var item = await _demoService.GetByIdAsync(id);
        if (item == null)
            return NotFound();
        return Ok(item);
    }

    /// <summary>
    /// Creates a demo item.
    /// </summary>
    [HttpPost("items")]
    public async Task<ActionResult<DemoItem>> CreateItem([FromBody] CreateDemoItemRequest request)
    {
        var item = await _demoService.CreateAsync(request.Key, request.Value);
        return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
    }

    /// <summary>
    /// Gets current UTC time and singleton instance ID (from [BonSingleton] AppClockService).
    /// </summary>
    [HttpGet("clock")]
    public ActionResult<object> GetClock()
    {
        return Ok(new
        {
            UtcNow = _clockService.UtcNow,
            InstanceId = _clockService.InstanceId,
            Source = "BonSingleton attribute"
        });
    }

    /// <summary>
    /// Gets next IDs (from [BonTransient] IdGeneratorService).
    /// </summary>
    [HttpGet("ids")]
    public ActionResult<object> GetIds()
    {
        return Ok(new
        {
            NextInt = _idGenerator.Next(),
            NextString = _idGenerator.NextString(),
            Source = "BonTransient attribute"
        });
    }

    /// <summary>
    /// Health check for attributed services.
    /// </summary>
    [HttpGet("health")]
    public ActionResult<object> Health()
    {
        return Ok(new
        {
            Status = "ok",
            Message = "Attributed services (BonScoped, BonSingleton, BonTransient) are registered and resolved.",
            Timestamp = _clockService.UtcNow
        });
    }
}

/// <summary>
/// Request model for creating a demo item.
/// </summary>
public class CreateDemoItemRequest
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
