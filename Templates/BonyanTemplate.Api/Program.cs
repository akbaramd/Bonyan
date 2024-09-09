using BonyanTemplate.Application.Jobs;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Domain.Repositories;
using BonyanTemplate.Infrastructure.Data;
using BonyanTemplate.Infrastructure.Data.Repositories;
using BonyanTemplate.Persistence.Seeds;
using Microsoft.EntityFrameworkCore;

BonyanApplication 
  .CreateBuilder("BonyanTemplate", "BonyanTemplate", "1.0.0", args)
  .AddFastEndpoints()
  .AddPersistence(presisence =>
  {

    presisence.AddEntityFrameworkCore<BonyanTemplateDbContext>(ef =>
    {
      ef.AddRepository<IBooksRepository,EfBookRepository>();
    });

    presisence.AddInMemory(im =>
    {
      im.AddRepository<IBooksRepository>();
      im.AddRepository<IBooksRepository,MemoryBookRepository>();
    });
    
    presisence.AddSeed<BookSeed>();
  })
  .AddCronJob<TestJob>("0 */6 * * *")
  .Build()
  .UseFastEndpoints()
  .Run();
