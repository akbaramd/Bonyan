using Bonyan.EntityFrameworkCore.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.EntityFrameworkCore;

public interface IBonDbContextProvider<TDbContext>
  where TDbContext : IEfDbContext
{
  Task<TDbContext> GetDbContextAsync();
}