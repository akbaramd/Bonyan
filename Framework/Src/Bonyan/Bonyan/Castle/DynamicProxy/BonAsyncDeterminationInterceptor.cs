using Bonyan.DependencyInjection;
using Castle.DynamicProxy;

namespace Bonyan.Castle.DynamicProxy;

    public class BonAsyncDeterminationInterceptor<TInterceptor> : AsyncDeterminationInterceptor
        where TInterceptor : IBonInterceptor
    {
        public BonAsyncDeterminationInterceptor(TInterceptor bonyanInterceptor)
            : base(new BonCastleAsyncInterceptorAdapter<TInterceptor>(bonyanInterceptor))
        {

        }
    }
