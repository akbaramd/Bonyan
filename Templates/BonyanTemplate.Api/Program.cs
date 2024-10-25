using Bonyan.FastEndpoints;
using BonyanTemplate.Api;
using BonyanTemplate.Application.Jobs;
using BonyanTemplate.Domain.Repositories;
using BonyanTemplate.Infrastructure.Data;
using BonyanTemplate.Infrastructure.Data.Repositories;
using FastEndpoints;
using Hangfire;

// create bonyan application builder
var builder = BonyanApplication.CreateApplicationBuilder<BonyanTemplateModule>(args);
var app = builder.Build();


app.Run();
