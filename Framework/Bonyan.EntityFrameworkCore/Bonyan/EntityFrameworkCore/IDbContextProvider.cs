using Bonyan.EntityFrameworkCore.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.EntityFrameworkCore;

public interface IDbContextProvider<TDbContext>
  where TDbContext : DbContext, IBonyanDbContext<TDbContext>
{
  Task<TDbContext> GetDbContextAsync();
}