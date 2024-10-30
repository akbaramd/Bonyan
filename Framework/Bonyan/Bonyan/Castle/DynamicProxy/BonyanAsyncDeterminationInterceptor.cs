using Bonyan.DependencyInjection;
using Castle.DynamicProxy;

namespace Bonyan.Castle.DynamicProxy;

    public class BonyanAsyncDeterminationInterceptor<TInterceptor> : AsyncDeterminationInterceptor
        where TInterceptor : IBonyanInterceptor
    {
        public BonyanAsyncDeterminationInterceptor(TInterceptor bonyanInterceptor)
            : base(new CastleAsyncBonyanInterceptorAdapter<TInterceptor>(bonyanInterceptor))
        {

        }
    }
