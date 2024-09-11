using BonyanTemplate.Application.Jobs;
using BonyanTemplate.Domain.Repositories;
using BonyanTemplate.Infrastructure.Data;
using BonyanTemplate.Infrastructure.Data.Repositories;
using BonyanTemplate.Persistence.Seeds;

// create bonyan application builder
var builder = BonyanApplication.CreateApplicationBuilder(args);

// configure builder
builder.AddFastEndpoints(fe =>
{
  fe.AddAuthentication(auth =>
  {
    auth.SigningKey = "ewelkdnaskjdbalkjdhn;aksdf;akjsdsdasdasd";
  });
});

builder.AddPersistence(persistence =>
{
  persistence.EnableTenant();
  persistence.AddEntityFrameworkCore<BonyanTemplateDbContext>(ef =>
  {
    ef.AddRepository<IBooksRepository, EfBookRepository>();
  });

  persistence.AddInMemory(im =>
  {
    im.AddRepository<IBooksRepository, MemoryBookRepository>();
  });

  persistence.AddSeed<BookSeed>();
});

builder.AddCronJob<TestJob>("0 */6 * * *");

// build application
var app = builder.Build();

// configure application
app.UseFastEndpoints();

// run application
app.Run();
