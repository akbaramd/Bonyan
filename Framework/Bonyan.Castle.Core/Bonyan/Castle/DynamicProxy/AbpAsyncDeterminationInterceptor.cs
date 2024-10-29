using Bonyan.DependencyInjection;
using Castle.DynamicProxy;

namespace Bonyan.Castle.DynamicProxy;

    public class AbpAsyncDeterminationInterceptor<TInterceptor> : AsyncDeterminationInterceptor
        where TInterceptor : IBonyanInterceptor
    {
        public AbpAsyncDeterminationInterceptor(TInterceptor abpInterceptor)
            : base(new CastleAsyncAbpInterceptorAdapter<TInterceptor>(abpInterceptor))
        {

        }
    }
