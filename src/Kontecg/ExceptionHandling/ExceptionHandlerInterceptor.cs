using System;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Kontecg.Aspects;
using Kontecg.Dependency;
using Kontecg.Events.Bus;
using Kontecg.Events.Bus.Exceptions;
using Kontecg.Extensions;

namespace Kontecg.ExceptionHandling
{
    public class ExceptionHandlerInterceptor : KontecgInterceptorBase, ITransientDependency
    {
        private readonly IExceptionHandlingConfiguration _exceptionHandlingConfiguration;
        private readonly IEventBus _eventBus;

        public ExceptionHandlerInterceptor(
            IExceptionHandlingConfiguration exceptionHandlingConfiguration,
            IEventBus eventBus)
        {
            _exceptionHandlingConfiguration = exceptionHandlingConfiguration;
            _eventBus = eventBus;
        }

        public override void InterceptSynchronous(IInvocation invocation)
        {
            if (KontecgCrossCuttingConcerns.IsApplied(invocation.InvocationTarget, KontecgCrossCuttingConcerns.ExceptionHandling))
            {
                invocation.Proceed();
                return;
            }

            if (!_exceptionHandlingConfiguration.IsEnabled)
            {
                invocation.Proceed();
                return;
            }

            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                if (_exceptionHandlingConfiguration.SendDetailedExceptionsToSupport)
                {

                }

                if (_exceptionHandlingConfiguration.TriggerDomainEvents)
                    _eventBus.Trigger(ProxyUtil.GetUnproxiedInstance(invocation.InvocationTarget), new KontecgHandledExceptionData(ex));

                if(_exceptionHandlingConfiguration.PropagatedHandledExceptions)
                    ex.ReThrow();
            }
        }

        protected override async Task InternalInterceptAsynchronousAsync(IInvocation invocation)
        {
            var proceedInfo = invocation.CaptureProceedInfo();

            if (KontecgCrossCuttingConcerns.IsApplied(invocation.InvocationTarget, KontecgCrossCuttingConcerns.ExceptionHandling))
            {
                proceedInfo.Invoke();
                var task = (Task)invocation.ReturnValue;
                await task;
                return;
            }

            if (!_exceptionHandlingConfiguration.IsEnabled)
            {
                proceedInfo.Invoke();
                var task = (Task)invocation.ReturnValue;
                await task;
                return;
            }

            try
            {
                proceedInfo.Invoke();
                var task = (Task)invocation.ReturnValue;
                await task;
            }
            catch (Exception ex)
            {
                if (_exceptionHandlingConfiguration.TriggerDomainEvents)
                    await _eventBus.TriggerAsync(ProxyUtil.GetUnproxiedInstance(invocation.InvocationTarget),
                        new KontecgHandledExceptionData(ex));

                if (_exceptionHandlingConfiguration.PropagatedHandledExceptions)
                    ex.ReThrow();
            }
        }

        protected override async Task<TResult> InternalInterceptAsynchronousAsync<TResult>(IInvocation invocation)
        {
            var proceedInfo = invocation.CaptureProceedInfo();
            if (KontecgCrossCuttingConcerns.IsApplied(invocation.InvocationTarget, KontecgCrossCuttingConcerns.ExceptionHandling))
            {
                proceedInfo.Invoke();
                var taskResult = (Task<TResult>)invocation.ReturnValue;
                return await taskResult;
            }

            if (!_exceptionHandlingConfiguration.IsEnabled)
            {
                proceedInfo.Invoke();
                var taskResult = (Task<TResult>)invocation.ReturnValue;
                return await taskResult;
            }

            TResult result = default;

            try
            {
                proceedInfo.Invoke();
                var taskResult = (Task<TResult>)invocation.ReturnValue;
                result = await taskResult;
            }
            catch (Exception ex)
            {
                if (_exceptionHandlingConfiguration.TriggerDomainEvents)
                    await _eventBus.TriggerAsync(ProxyUtil.GetUnproxiedInstance(invocation.InvocationTarget),
                        new KontecgHandledExceptionData(ex));

                if (_exceptionHandlingConfiguration.PropagatedHandledExceptions)
                    ex.ReThrow();
            }

            return result;
        }
    }
}
