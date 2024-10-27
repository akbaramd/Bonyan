using Bonyan.DomainDrivenDesign.Application;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Modularity.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Bonyan.AspNetCore.Persistence.EntityFrameworkCore
{
  [DependOn(typeof(BonyanPersistenceModule))]
  public class BonyanPersistenceEntityFrameworkModule : Module
  {
      public override Task OnConfigureAsync(ModularityContext context)
      {
         
          
          return base.OnConfigureAsync(context);
      }
  }
}
