using Bonyan.Demo.Api.Jobs;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Domain.Repositories;
using BonyanTemplate.Persistence;
using BonyanTemplate.Persistence.Seeds;
using Microsoft.EntityFrameworkCore;

BonyanApplication
  .CreateBuilder("Demo", "demo", "1.0.0", args)
  .AddFastEndpoints()
  .AddPersistence(c =>
  {
    c.AddEntityFrameworkCore<AppDbContext>(efCore =>
    {
      efCore.AddRepository<IBooksRepository>();
      
    });
    c.AddSeed<BookSeed>();

  })
  .AddCronJob<DemoJobs>("*/1 * * * *")
  .Build()
  .UseFastEndpoints()
  .Run();
