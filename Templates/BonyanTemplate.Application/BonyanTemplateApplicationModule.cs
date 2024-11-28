using Bonyan.AutoMapper;
using Bonyan.IdentityManagement.Application;
using Bonyan.IdentityManagement.Application.Dto;
using Bonyan.IdentityManagement.Domain;
using Bonyan.Messaging;
using Bonyan.Messaging.RabbitMQ;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.TenantManagement.Application;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Bonyan.Workers;
using Bonyan.Workers.Hangfire;
using BonyanTemplate.Application.Authors;
using BonyanTemplate.Application.Authors.Dtos;
using BonyanTemplate.Application.Books;
using BonyanTemplate.Application.Books.Dtos;
using BonyanTemplate.Application.Books.Jobs;
using BonyanTemplate.Application.Consumers;
using BonyanTemplate.Domain;
using BonyanTemplate.Domain.Authors;
using BonyanTemplate.Domain.Users;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;


namespace BonyanTemplate.Application
{
    public class BonyanTemplateApplicationModule : BonModule
    {
        public BonyanTemplateApplicationModule()
        {
            DependOn<BonTenantManagementApplicationModule>();
            DependOn<BonIdentityManagementApplicationModule<User>>();
            DependOn<BonyanTemplateDomainModule>();
            DependOn<BonWorkersHangfireModule>();
        }

        public override Task OnConfigureAsync(BonConfigurationContext context)
        {
            PreConfigure<BonMessagingConfiguration>(c =>
            {
                c.RegisterConsumer<BookConsumer>("");
            });
            
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


        public override async Task OnInitializeAsync(BonInitializedContext context)
        {
            var service = context.RequireService<IBonIdentityAuthService>();
            var res = await service.RegisterAsync(new BonIdentityUserRegistererDto()
            {
                Email = new BonUserEmail("akbarsafari00@gmail.com"),
                Password = "Aa@13567975",
                Status = UserStatus.Active,
                PhoneNumber = new BonUserPhoneNumber("09371770774"),
                UserName = "akbarsfari00"
            });
            var login = await service.JwtBearerSignInAsync("akbarsfari00", "Aa@13567975");
            await base.OnInitializeAsync(context);
        }
    }
}