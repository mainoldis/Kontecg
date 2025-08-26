using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Kontecg.Dependency;

namespace Kontecg.Domain.Uow
{
    /// <summary>
    /// This interceptor is used to manage database connection and transactions.
    /// </summary>
    internal class UnitOfWorkInterceptor : KontecgInterceptorBase, ITransientDependency
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IUnitOfWorkDefaultOptions _unitOfWorkOptions;

        public UnitOfWorkInterceptor(IUnitOfWorkManager unitOfWorkManager, IUnitOfWorkDefaultOptions unitOfWorkOptions)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _unitOfWorkOptions = unitOfWorkOptions;
        }

        public override void InterceptSynchronous(IInvocation invocation)
        {
            var method = GetMethodInfo(invocation);
            var unitOfWorkAttr = _unitOfWorkOptions.GetUnitOfWorkAttributeOrNull(method);

            if (unitOfWorkAttr == null || unitOfWorkAttr.IsDisabled)
            {
                invocation.Proceed();
                return;
            }

            using (var uow = _unitOfWorkManager.Begin(unitOfWorkAttr.CreateOptions()))
            {
                invocation.Proceed();
                uow.Complete();
            }
        }

        protected override async Task InternalInterceptAsynchronousAsync(IInvocation invocation)
        {
            var proceedInfo = invocation.CaptureProceedInfo();
            var method = GetMethodInfo(invocation);
            var unitOfWorkAttr = _unitOfWorkOptions.GetUnitOfWorkAttributeOrNull(method);

            if (unitOfWorkAttr == null || unitOfWorkAttr.IsDisabled)
            {
                proceedInfo.Invoke();
                var task = (Task)invocation.ReturnValue;
                await task;
                return;
            }

            using (var uow = _unitOfWorkManager.Begin(unitOfWorkAttr.CreateOptions()))
            {
                proceedInfo.Invoke();
                var task = (Task)invocation.ReturnValue;
                await task;
                await uow.CompleteAsync();
            }
        }


        protected override async Task<TResult> InternalInterceptAsynchronousAsync<TResult>(IInvocation invocation)
        {
            var proceedInfo = invocation.CaptureProceedInfo();
            var method = GetMethodInfo(invocation);
            var unitOfWorkAttr = _unitOfWorkOptions.GetUnitOfWorkAttributeOrNull(method);

            if (unitOfWorkAttr == null || unitOfWorkAttr.IsDisabled)
            {
                proceedInfo.Invoke();
                var taskResult = (Task<TResult>)invocation.ReturnValue;
                return await taskResult;
            }

            using (var uow = _unitOfWorkManager.Begin(unitOfWorkAttr.CreateOptions()))
            {
                proceedInfo.Invoke();
                
                var taskResult = (Task<TResult>)invocation.ReturnValue;
                var result = await taskResult;

                await uow.CompleteAsync();

                return result;
            }
        }

        private static MethodInfo GetMethodInfo(IInvocation invocation)
        {
            MethodInfo method;
            try
            {
                method = invocation.MethodInvocationTarget;
            }
            catch
            {
                method = invocation.GetConcreteMethod();
            }

            return method;
        }
    }
}
