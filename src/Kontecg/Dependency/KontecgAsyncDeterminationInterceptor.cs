using Castle.DynamicProxy;

namespace Kontecg.Dependency
{
    public class KontecgAsyncDeterminationInterceptor<TInterceptor> : AsyncDeterminationInterceptor
        where TInterceptor : IAsyncInterceptor
    {
        public KontecgAsyncDeterminationInterceptor(TInterceptor asyncInterceptor) : base(asyncInterceptor)
        {
        }
    }
}
