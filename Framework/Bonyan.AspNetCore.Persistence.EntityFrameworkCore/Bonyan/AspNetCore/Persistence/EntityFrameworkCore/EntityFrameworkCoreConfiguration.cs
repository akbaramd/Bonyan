using Bonyan.AspNetCore.Persistence.EntityFrameworkCore.HostedServices;
using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using Bonyan.DomainDrivenDesign.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.AspNetCore.Persistence.EntityFrameworkCore
{
  public class EntityFrameworkCoreConfiguration
  {
    public IServiceCollection Services { get; }
    public Action<DbContextOptionsBuilder> DbContextOptionsAction { get; set; } = _ => { };


    public EntityFrameworkCoreConfiguration Configure(Action<DbContextOptionsBuilder> action)
    {
      DbContextOptionsAction = action;
      return this;
    }

  }
}

