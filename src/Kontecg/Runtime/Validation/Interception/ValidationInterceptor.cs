using System.Threading.Tasks;
using Castle.DynamicProxy;
using Kontecg.Aspects;
using Kontecg.Dependency;

namespace Kontecg.Runtime.Validation.Interception
{
    /// <summary>
    ///     This interceptor is used intercept method calls for classes which's methods must be validated.
    /// </summary>
    public class ValidationInterceptor : KontecgInterceptorBase, ITransientDependency
    {
        private readonly IIocResolver _iocResolver;

        public ValidationInterceptor(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public override void InterceptSynchronous(IInvocation invocation)
        {
            if (KontecgCrossCuttingConcerns.IsApplied(invocation.InvocationTarget,
                    KontecgCrossCuttingConcerns.Validation))
            {
                invocation.Proceed();
                return;
            }

            using (IDisposableDependencyObjectWrapper<MethodInvocationValidator> validator =
                   _iocResolver.ResolveAsDisposable<MethodInvocationValidator>())
            {
                validator.Object.Initialize(invocation.MethodInvocationTarget, invocation.Arguments);
                validator.Object.Validate();
            }

            invocation.Proceed();
        }

        protected override async Task InternalInterceptAsynchronousAsync(IInvocation invocation)
        {
            IInvocationProceedInfo proceedInfo = invocation.CaptureProceedInfo();

            if (KontecgCrossCuttingConcerns.IsApplied(invocation.InvocationTarget,
                    KontecgCrossCuttingConcerns.Validation))
            {
                proceedInfo.Invoke();
                await (Task) invocation.ReturnValue;
                return;
            }

            using (IDisposableDependencyObjectWrapper<MethodInvocationValidator> validator =
                   _iocResolver.ResolveAsDisposable<MethodInvocationValidator>())
            {
                validator.Object.Initialize(invocation.MethodInvocationTarget, invocation.Arguments);
                validator.Object.Validate();
            }

            proceedInfo.Invoke();
            await (Task) invocation.ReturnValue;
        }

        protected override async Task<TResult> InternalInterceptAsynchronousAsync<TResult>(IInvocation invocation)
        {
            IInvocationProceedInfo proceedInfo = invocation.CaptureProceedInfo();

            if (KontecgCrossCuttingConcerns.IsApplied(invocation.InvocationTarget,
                    KontecgCrossCuttingConcerns.Validation))
            {
                proceedInfo.Invoke();
                return await (Task<TResult>) invocation.ReturnValue;
            }

            using (IDisposableDependencyObjectWrapper<MethodInvocationValidator> validator =
                   _iocResolver.ResolveAsDisposable<MethodInvocationValidator>())
            {
                validator.Object.Initialize(invocation.MethodInvocationTarget, invocation.Arguments);
                validator.Object.Validate();
            }

            proceedInfo.Invoke();
            return await (Task<TResult>) invocation.ReturnValue;
        }
    }
}
