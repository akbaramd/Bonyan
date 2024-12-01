using Bonyan.DependencyInjection;
using Bonyan.IdentityManagement.EntityFrameworkCore;
using Bonyan.Messaging;
using Bonyan.Messaging.OutBox;
using Bonyan.Messaging.RabbitMQ;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.MultiTenant;
using Bonyan.TenantManagement.EntityFrameworkCore;
using BonyanTemplate.Application.Consumers;
using BonyanTemplate.Domain;
using BonyanTemplate.Domain.Authors;
using BonyanTemplate.Domain.Books;
using BonyanTemplate.Domain.Users;
using BonyanTemplate.Infrastructure.Data;
using BonyanTemplate.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BonyanTemplate.Infrastructure;

public class BonaynTempalteInfrastructureModule : BonModule
{
    public BonaynTempalteInfrastructureModule()
    {
        DependOn<BonTenantManagementEntityFrameworkModule>();
        DependOn<BonIdentityManagementEntityFrameworkCoreModule<User>>();
        DependOn<BonyanTemplateDomainModule>();
        DependOn<BonMessagingRabbitMqModule>();
        DependOn<BonMessagingOutboxModule>();
        DependOn<BonMessagingOutboxEntityFrameworkModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        PreConfigure<BonMessagingConfiguration>(c =>
        {
            c.RegisterConsumer<BookConsumer>();
        });
        PreConfigure<BonRabbitMqConfiguration>(options =>
        {
            options.HostName = "localhost";
            options.Port = 5672;
            options.UserName = "guest";
            options.Password = "guest";
            options.VirtualHost = "/";
            options.ConfigureConsumer<BookConsumer>("test");
        });
        
        PreConfigure<BonMessagingOutBoxConfiguration>(options =>
        {
            options.UseEntityFrameworkCoreStore<BonyanTemplateDbContext>();
        });
        
        context.ConfigureOptions<BonMultiTenancyOptions>(options => { options.IsEnabled = true; });
        context.Services.AddTransient<IBooksRepository, EfBookRepository>();
        context.Services.AddTransient<IAuthorsBonRepository, EfAuthorBonRepository>();
        context.AddDbContext<BonyanTemplateDbContext>(c =>
        {
            c.UseSqlite("Data Source=BonyanTemplate.db");
            c.AddDefaultRepositories(true);
        });


        return base.OnConfigureAsync(context);
    }

 
}