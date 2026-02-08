using Bonyan.DependencyInjection;
using Castle.DynamicProxy;

namespace Bonyan.Castle.DynamicProxy;

/// <summary>
/// Castle wrapper that determines sync vs async methods and delegates to <see cref="BonCastleAsyncInterceptorAdapter{TInterceptor}"/>.
/// Used with Autofac <c>InterceptedBy&lt;BonAsyncDeterminationInterceptor&lt;TInterceptor&gt;&gt;</c>.
/// </summary>
/// <typeparam name="TInterceptor">The Bonyan interceptor type (must implement <see cref="IBonInterceptor"/>).</typeparam>
public class BonAsyncDeterminationInterceptor<TInterceptor> : AsyncDeterminationInterceptor
    where TInterceptor : IBonInterceptor
{
    /// <summary>
    /// Creates the determination interceptor wrapping the Bonyan interceptor.
    /// </summary>
    public BonAsyncDeterminationInterceptor(TInterceptor interceptor)
        : base(new BonCastleAsyncInterceptorAdapter<TInterceptor>(interceptor))
    {
    }
}
