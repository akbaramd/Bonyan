using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using Bonyan.FastEndpoints;
using BonyanTemplate.Api;
using BonyanTemplate.Application.Jobs;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Domain.Repositories;
using BonyanTemplate.Infrastructure.Data;
using BonyanTemplate.Infrastructure.Data.Repositories;
using FastEndpoints;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

// create bonyan application builder
var builder = BonyanApplication.CreateApplicationBuilder<BonyanTemplateModule>(args);
var app = builder.Build();

app.MapGet("/", async ([FromServices]IRepository<Books> book) =>
{
  return await book.FindAsync(x=>true);
});
app.Run();
