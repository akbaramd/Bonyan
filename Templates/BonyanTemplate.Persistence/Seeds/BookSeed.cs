using System.Linq.Expressions;
using Bonyan.EntityFrameworkCore;
using BonyanTemplate.Domain.Entities;

namespace BonyanTemplate.Persistence.Seeds;

public class BookSeed : SeedBase
{


  public override Task SeedAsync()
  {
    return Task.CompletedTask;
  }
}
