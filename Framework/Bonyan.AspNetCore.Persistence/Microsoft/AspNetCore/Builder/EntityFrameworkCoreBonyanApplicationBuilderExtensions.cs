

using Bonyan.AspNetCore.Persistence;
using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using Bonyan.DomainDrivenDesign.Domain.Core;

namespace Microsoft.AspNetCore.Builder;

public  static class EntityFrameworkCoreBonyanApplicationBuilderExtensions
{
  public static IBonyanApplicationBuilder AddPersistence(this IBonyanApplicationBuilder applicationBuilder,Action<PersistenceConfiguration> configure)
  {
  
    
    var conf = new PersistenceConfiguration(applicationBuilder);
    configure.Invoke(conf);

    applicationBuilder.AddBackgroundJob<SeedBackgroundJobs>();
    
    return applicationBuilder;
  }
  
}