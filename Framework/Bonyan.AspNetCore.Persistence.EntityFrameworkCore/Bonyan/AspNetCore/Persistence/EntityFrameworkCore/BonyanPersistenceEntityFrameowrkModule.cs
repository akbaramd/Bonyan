using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Modularity.Attributes;
using Microsoft.Extensions.Options;

namespace Bonyan.AspNetCore.Persistence.EntityFrameworkCore
{
  [DependOn(typeof(BonyanPersistenceModule))]
  public class BonyanPersistenceEntityFrameowrkModule : Module
  {

  }
}
