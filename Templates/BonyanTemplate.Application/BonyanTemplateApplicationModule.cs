using System.Data;
using Bonyan.AutoMapper;
using Bonyan.IdentityManagement;
using Bonyan.IdentityManagement.Application;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.UnitOfWork;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Bonyan.Workers;
using Bonyan.Workers.Hangfire;
using BonyanTemplate.Application.Authors;
using BonyanTemplate.Application.Books;
using BonyanTemplate.Application.Books.Jobs;
using BonyanTemplate.Domain;
using BonyanTemplate.Domain.Users;
using Microsoft.Extensions.DependencyInjection;


namespace BonyanTemplate.Application
{
    public class BonyanTemplateApplicationModule : BonModule
    {
        public BonyanTemplateApplicationModule()
        {
            DependOn<BonTenantManagementApplicationModule>();
            DependOn<BonIdentityManagementApplicationModule<User,Role>>();
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