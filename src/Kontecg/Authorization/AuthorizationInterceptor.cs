using System.Threading.Tasks;
using Castle.DynamicProxy;
using Kontecg.Dependency;
using Kontecg.Events.Bus;
using Kontecg.Events.Bus.Exceptions;
using Kontecg.ExceptionHandling;
using Kontecg.Extensions;

namespace Kontecg.Authorization
{
    /// <summary>
    ///     This class is used to intercept methods to make authorization if the method defined
    ///     <see cref="KontecgAuthorizeAttribute" />.
    /// </summary>
    public class AuthorizationInterceptor : KontecgInterceptorBase, ITransientDependency
    {
        private readonly IAuthorizationHelper _authorizationHelper;
        private readonly IExceptionHandlingConfiguration _exceptionHandlingConfiguration;
        private readonly IEventBus _eventBus;

        public AuthorizationInterceptor(
            IAuthorizationHelper authorizationHelper,
            IExceptionHandlingConfiguration exceptionHandlingConfiguration,
            IEventBus eventBus)
        {
            _authorizationHelper = authorizationHelper;
            _exceptionHandlingConfiguration = exceptionHandlingConfiguration;
            _eventBus = eventBus;
        }

        public void Intercept(IInvocation invocation)
        {
            try
            {
                _authorizationHelper.Authorize(invocation.MethodInvocationTarget, invocation.TargetType);
                invocation.Proceed();
            }
            catch (KontecgAuthorizationException e)
            {
                HandleAuthorization(invocation, e);
            }
        }

        public override void InterceptSynchronous(IInvocation invocation)
        {
            try
            {
                _authorizationHelper.Authorize(invocation.MethodInvocationTarget, invocation.TargetType);
                invocation.Proceed();
            }
            catch (KontecgAuthorizationException e)
            {
                HandleAuthorization(invocation, e);
            }
        }

        protected override async Task InternalInterceptAsynchronousAsync(IInvocation invocation)
        {
            IInvocationProceedInfo proceedInfo = invocation.CaptureProceedInfo();

            try
            {
                await _authorizationHelper.AuthorizeAsync(invocation.MethodInvocationTarget, invocation.TargetType);

                proceedInfo.Invoke();
                Task task = (Task) invocation.ReturnValue;
                await task;
            }
            catch (KontecgAuthorizationException e)
            {
                HandleAuthorization(invocation, e);
            }
        }

        protected override async Task<TResult> InternalInterceptAsynchronousAsync<TResult>(IInvocation invocation)
        {
            IInvocationProceedInfo proceedInfo = invocation.CaptureProceedInfo();
            TResult result = default;
            try
            {
                await _authorizationHelper.AuthorizeAsync(invocation.MethodInvocationTarget, invocation.TargetType);

                proceedInfo.Invoke();
                Task<TResult> taskResult = (Task<TResult>)invocation.ReturnValue;
                result = await taskResult;
            }
            catch (KontecgAuthorizationException e)
            {
                if (!_exceptionHandlingConfiguration.IsEnabled)
                    e.ReThrow();

                HandleAuthorization(invocation, e);
            }

            return result;
        }

        protected virtual void HandleAuthorization(IInvocation invocation, KontecgAuthorizationException exception)
        {
            if (_exceptionHandlingConfiguration.TriggerDomainEvents)
                _eventBus.Trigger(ProxyUtil.GetUnproxiedInstance(invocation.InvocationTarget),
                    new KontecgHandledExceptionData(exception));

            if (_exceptionHandlingConfiguration.PropagatedHandledExceptions)
                exception.ReThrow();
        }
    }
}
