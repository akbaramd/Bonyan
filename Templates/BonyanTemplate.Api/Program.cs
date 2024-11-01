using Bonyan.Layer.Domain.Abstractions;
using Bonyan.TenantManagement.Domain;
using BonyanTemplate.Api;
using BonyanTemplate.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

// create bonyan application builder
var builder = BonyanApplication.CreateApplicationBuilder<BonyanTemplateModule>(args);


var app = await builder.BuildAsync();



app.MapGet("/", async ([FromServices]IRepository<Books> book) =>
{
  return await book.FindAsync(x=>true);
}).RequireAuthorization();

var s = app.Services.GetRequiredService<ITenantManager>();

app.Run();


