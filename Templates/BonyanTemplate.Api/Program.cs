using BonyanTemplate.Application.Jobs;
using BonyanTemplate.Domain.Repositories;
using BonyanTemplate.Infrastructure.Data;
using BonyanTemplate.Infrastructure.Data.Repositories;
using BonyanTemplate.Persistence.Seeds;

BonyanApplication
  .CreateBuilder("BonyanTemplate", "BonyanTemplate", "1.0.0", args)
  .AddFastEndpoints(c =>
  {
    c.AddAuthentication(c =>
    {
      c.SigningKey = "";
    });
  })
  .AddPersistence(presisence =>
  {
    presisence.AddEntityFrameworkCore<BonyanTemplateDbContext>(ef =>
    {
      ef.AddRepository<IBooksRepository, EfBookRepository>();
    });

    presisence.AddInMemory(im =>
    {
      im.AddRepository<IBooksRepository>();
      im.AddRepository<IBooksRepository, MemoryBookRepository>();
    });

    presisence.AddSeed<BookSeed>();
  })
  .AddCronJob<TestJob>("0 */6 * * *")
  .Build()
  .UseFastEndpoints()
  .Run();
