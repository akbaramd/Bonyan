using Bonyan.AutoMapper;
using Bonyan.Messaging;
using Bonyan.Messaging.RabbitMQ;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.TenantManagement.Application;
using Bonyan.Worker;
using Bonyan.Workers.Hangfire;
using BonyanTemplate.Application.Dtos;
using BonyanTemplate.Application.Jobs;
using BonyanTemplate.Domain;
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
            DependOn<BonWorkersModule>();
            DependOn<BonMessagingModule>();
            DependOn<BonMessagingRabbitMQModule>();
        }

        public override Task OnPreConfigureAsync(BonConfigurationContext context)
        {
            PreConfigure<IGlobalConfiguration>(c => { c.ToString(); });
            PreConfigure<RabbitMQOptions>(c =>
            {
                
            });


            return base.OnPreConfigureAsync(context);
        }

        public override Task OnConfigureAsync(BonConfigurationContext context)
        {
            context.ConfigureOptions<BonAutoMapperOptions>(options => { options.AddProfile<BookMapper>(); });

            PreConfigure<BonWorkerConfiguration>(c => { c.RegisterWorker<TestBonWorker>(); });

            return base.OnConfigureAsync(context);
        }


        public override Task OnInitializeAsync(BonInitializedContext context)
        {
            // context.AddBackgroundWorkerAsync<TestBonWorker>();
            return base.OnInitializeAsync(context);
        }
    }
}