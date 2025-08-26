using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace Kontecg.Dependency
{
    public abstract class KontecgInterceptorBase : IAsyncInterceptor
    {
        public virtual void InterceptAsynchronous(IInvocation invocation)
        {
            invocation.ReturnValue = InternalInterceptAsynchronousAsync(invocation);
        }

        public virtual void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            invocation.ReturnValue = InternalInterceptAsynchronousAsync<TResult>(invocation);
        }

        public abstract void InterceptSynchronous(IInvocation invocation);

        protected abstract Task InternalInterceptAsynchronousAsync(IInvocation invocation);

        protected abstract Task<TResult> InternalInterceptAsynchronousAsync<TResult>(IInvocation invocation);
    }
}
