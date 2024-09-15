using Bonyan.FastEndpoints;
using BonyanTemplate.Application.Jobs;
using BonyanTemplate.Domain.Repositories;
using BonyanTemplate.Infrastructure.Data;
using BonyanTemplate.Infrastructure.Data.Repositories;
using BonyanTemplate.Persistence.Seeds;

// create bonyan application builder
var builder = BonyanApplication.CreateApplicationBuilder(args);

builder.

builder.AddCronJob<TestJob>("0 */6 * * *");

// build application
var app = builder.Build();

// run application
app.Run();
