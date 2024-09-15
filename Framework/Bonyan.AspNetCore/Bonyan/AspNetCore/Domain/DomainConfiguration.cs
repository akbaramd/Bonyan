namespace Bonyan.AspNetCore.Domain;

public class DomainConfiguration(IConfiguration configuration) : IDomainConfiguration
{
  public IConfiguration Configuration { get; set; } = configuration;
}
