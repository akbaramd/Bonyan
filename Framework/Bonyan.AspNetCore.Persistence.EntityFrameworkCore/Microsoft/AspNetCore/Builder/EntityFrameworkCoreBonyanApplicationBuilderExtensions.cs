


using Bonyan.AspNetCore.Persistence;
using Bonyan.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.AspNetCore.Builder;

public  static class EntityFrameworkCoreBonyanApplicationBuilderExtensions
{
  public static PersistenceConfiguration AddEntityFrameworkCore<TDbContext>(this PersistenceConfiguration conf,Action<EfCoreConfiguration<TDbContext>> configure) where TDbContext : DbContext
  {
    
    
    var configuration = new EfCoreConfiguration<TDbContext>(conf.Builder.Services);
    configure(configuration);
    
    conf.Builder.Services.AddDbContext<TDbContext>(configuration.DbContextOptionsAction);
    // Optionally run seeds after the app starts
    return conf;
  }
}
