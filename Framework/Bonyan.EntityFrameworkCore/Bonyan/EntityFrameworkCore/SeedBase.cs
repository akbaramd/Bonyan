using System.Linq.Expressions;
using Bonyan.DDD.Domain.Entities;

namespace Bonyan.EntityFrameworkCore;

public abstract class SeedBase
{
 
  public abstract Task SeedAsync();
}
