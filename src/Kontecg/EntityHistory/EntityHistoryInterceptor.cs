using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Kontecg.Dependency;

namespace Kontecg.EntityHistory
{
    internal class EntityHistoryInterceptor : KontecgInterceptorBase, ITransientDependency
    {
        public EntityHistoryInterceptor()
        {
            ReasonProvider = NullEntityChangeSetReasonProvider.Instance;
        }

        public IEntityChangeSetReasonProvider ReasonProvider { get; set; }

        public override void InterceptSynchronous(IInvocation invocation)
        {
            MethodInfo methodInfo = invocation.MethodInvocationTarget;
            UseCaseAttribute useCaseAttribute =
                methodInfo.GetCustomAttributes(true).OfType<UseCaseAttribute>().FirstOrDefault()
                ?? methodInfo.DeclaringType.GetCustomAttributes(true).OfType<UseCaseAttribute>()
                    .FirstOrDefault();

            if (useCaseAttribute?.Description == null)
            {
                invocation.Proceed();
                return;
            }

            using (ReasonProvider.Use(useCaseAttribute.Description))
            {
                invocation.Proceed();
            }
        }

        protected override async Task InternalInterceptAsynchronousAsync(IInvocation invocation)
        {
            IInvocationProceedInfo proceedInfo = invocation.CaptureProceedInfo();

            MethodInfo methodInfo = invocation.MethodInvocationTarget;
            UseCaseAttribute useCaseAttribute =
                methodInfo.GetCustomAttributes(true).OfType<UseCaseAttribute>().FirstOrDefault()
                ?? methodInfo.DeclaringType.GetCustomAttributes(true).OfType<UseCaseAttribute>()
                    .FirstOrDefault();

            if (useCaseAttribute?.Description == null)
            {
                proceedInfo.Invoke();
                Task task = (Task) invocation.ReturnValue;
                await task;
                return;
            }

            using (ReasonProvider.Use(useCaseAttribute.Description))
            {
                proceedInfo.Invoke();
                Task task = (Task) invocation.ReturnValue;
                await task;
            }
        }

        protected override async Task<TResult> InternalInterceptAsynchronousAsync<TResult>(IInvocation invocation)
        {
            IInvocationProceedInfo proceedInfo = invocation.CaptureProceedInfo();

            MethodInfo methodInfo = invocation.MethodInvocationTarget;
            UseCaseAttribute useCaseAttribute =
                methodInfo.GetCustomAttributes(true).OfType<UseCaseAttribute>().FirstOrDefault()
                ?? methodInfo.DeclaringType.GetCustomAttributes(true).OfType<UseCaseAttribute>()
                    .FirstOrDefault();

            if (useCaseAttribute?.Description == null)
            {
                proceedInfo.Invoke();
                Task<TResult> taskResult = (Task<TResult>) invocation.ReturnValue;
                return await taskResult;
            }

            using (ReasonProvider.Use(useCaseAttribute.Description))
            {
                proceedInfo.Invoke();
                Task<TResult> taskResult = (Task<TResult>) invocation.ReturnValue;
                return await taskResult;
            }
        }
    }
}
