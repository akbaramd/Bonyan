    using Bonyan.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    namespace Bonyan.UnitOfWork;

    public class BonUnitOfWorkInterceptor : BonInterceptor
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public BonUnitOfWorkInterceptor(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public override async Task InterceptAsync(IBonMethodInvocation invocation)
        {
            if (!BonUnitOfWorkHelper.IsUnitOfWorkMethod(invocation.Method, out var unitOfWorkAttribute))
            {
                await invocation.ProceedAsync();
                return;
            }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var options = CreateOptions(scope.ServiceProvider, invocation, unitOfWorkAttribute);

                var unitOfWorkManager = scope.ServiceProvider.GetRequiredService<IBonUnitOfWorkManager>();

                //Trying to begin a reserved UOW by BonUnitOfWorkMiddleware
                if (unitOfWorkManager.TryBeginReserved(BonUnitOfWork.UnitOfWorkReservationName, options))
                {
                    await invocation.ProceedAsync();

                    if (unitOfWorkManager.Current != null)
                    {
                        await unitOfWorkManager.Current.SaveChangesAsync();
                    }

                    return;
                }

                using (var uow = unitOfWorkManager.Begin(options))
                {
                    await invocation.ProceedAsync();
                    await uow.CompleteAsync();
                }
            }
        }

        private BonUnitOfWorkOptions CreateOptions(IServiceProvider serviceProvider, IBonMethodInvocation invocation, BonUnitOfWorkAttribute? unitOfWorkAttribute)
        {
            var options = new BonUnitOfWorkOptions();

            unitOfWorkAttribute?.SetOptions(options);

            if (unitOfWorkAttribute?.IsTransactional == null)
            {
                var defaultOptions = serviceProvider.GetRequiredService<IOptions<BonUnitOfWorkDefaultOptions>>().Value;
                options.IsTransactional = defaultOptions.CalculateIsTransactional(
                    autoValue: serviceProvider.GetRequiredService<IUnitOfWorkTransactionBehaviourProvider>().IsTransactional
                               ?? !invocation.Method.Name.StartsWith("Get", StringComparison.InvariantCultureIgnoreCase)
                );
            }

            return options;
        }
    }
