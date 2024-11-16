using Microsoft.EntityFrameworkCore;

namespace Bonyan.EntityFrameworkCore.Abstractions;

public interface IBonDbContext<TDbContext> : IBonEfCoreDbContext where TDbContext: DbContext
{
  
}
