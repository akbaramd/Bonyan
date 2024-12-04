using System.Data;
using Bonyan.AutoMapper;
using Bonyan.IdentityManagement;
using Bonyan.IdentityManagement.Application;
using Bonyan.IdentityManagement.Application.Auth;
using Bonyan.IdentityManagement.Application.Dto;
using Bonyan.IdentityManagement.Application.Roles;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.TenantManagement.Application;
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
            DependOn<BonIdentityManagementApplicationModule<User>>();
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


        public override async Task OnInitializeAsync(BonInitializedContext context)
        {
           
            var unitOfWork = context.RequireService<IBonUnitOfWorkManager>();
            
            var roleService = context.RequireService<IBonIdentityRoleAppService>();
            var service = context.RequireService<IBonIdentityUserAppService>();

            using (var uow = unitOfWork.Begin(new BonUnitOfWorkOptions()
                   {
                       IsTransactional = false,
                       IsolationLevel = IsolationLevel.ReadCommitted
                   }))
            {
                var adminRoleResult = await roleService.CreateAsync(new BonIdentityRoleCreateDto()
                {
                    Key = "admin",
                    Title = "admin",
                    Permissions = [
                        BonIdentityPermissionConstants.IdentityPermissionRead,
                        BonIdentityPermissionConstants.IdentityRoleRead,
                        BonIdentityPermissionConstants.IdentityRoleDelete,
                        BonIdentityPermissionConstants.IdentityRoleEdit,
                        BonIdentityPermissionConstants.IdentityRoleCreate,
                        BonIdentityPermissionConstants.IdentityUserRead,
                        BonIdentityPermissionConstants.IdentityUserDelete,
                        BonIdentityPermissionConstants.IdentityUserEdit,
                        BonIdentityPermissionConstants.IdentityUserCreate
                    ]
                });
            
      
                var userCreatedResult = await service.CreateAsync(new BonIdentityUserCreateDto()
                {
                    Password = "Aa@13567975",
                    PhoneNumber = new BonUserPhoneNumber("09371770774"),
                    Email = new BonUserEmail("akbarsafari00@gmail.com"),
                    UserName = "akbarsafari00",
                    Roles = ["admin"],
                    Status = UserStatus.IdActive
                });
                await uow.CompleteAsync();
            }
            
         

            await base.OnInitializeAsync(context);
        }
    }
}