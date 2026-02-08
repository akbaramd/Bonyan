namespace WebApi.Demo.Demos.Services;

/// <summary>
/// Id generator for testing [BonTransient] registration.
/// </summary>
public interface IIdGeneratorService
{
    int Next();
    string NextString();
}
