using Bonyan.AutoMapper;
using Bonyan.Messaging;
using Bonyan.Messaging.RabbitMQ;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.TenantManagement.Application;
using Bonyan.Workers;
using Bonyan.Workers.Hangfire;
using BonyanTemplate.Application.Authors;
using BonyanTemplate.Application.Authors.Dtos;
using BonyanTemplate.Application.Books;
using BonyanTemplate.Application.Books.Dtos;
using BonyanTemplate.Application.Books.Jobs;
using BonyanTemplate.Domain;
using BonyanTemplate.Domain.Authors;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;


namespace BonyanTemplate.Application
{
    public class BonyanTemplateApplicationModule : BonModule
    {
        public BonyanTemplateApplicationModule()
        {
            DependOn<BonTenantManagementApplicationModule>();
            DependOn<BonyanTemplateDomainModule>();
            DependOn<BonWorkersHangfireModule>();
        }

        public override Task OnConfigureAsync(BonConfigurationContext context)
        {
            context.Services.AddTransient<IBookAppService, BookAppService>();
            context.Services.AddTransient<IAuthorAppService, AuthorAppService>();

            context.ConfigureOptions<BonAutoMapperOptions>(options =>
            {
                options.AddProfile<BookMapper>();
                options.AddProfile<AuthorMapper>();
            });

            PreConfigure<BonWorkerConfiguration>(c => { c.RegisterWorker<BookOutOfStockNotifierWorker>(); });

            return base.OnConfigureAsync(context);
        }
        

    }
}