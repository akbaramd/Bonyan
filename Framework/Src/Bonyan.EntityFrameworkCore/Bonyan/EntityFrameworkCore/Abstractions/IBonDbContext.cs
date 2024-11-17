using Microsoft.EntityFrameworkCore;

namespace Bonyan.EntityFrameworkCore.Abstractions;

public interface IBonDbContext<TDbContext> : IEfDbContext where TDbContext: DbContext
{
  
}
