using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Kontecg.Aspects;
using Kontecg.Dependency;

namespace Kontecg.Auditing
{
    internal class AuditingInterceptor : KontecgInterceptorBase, ITransientDependency
    {
        private readonly IAuditingConfiguration _auditingConfiguration;
        private readonly IAuditingHelper _auditingHelper;
        private readonly IAuditSerializer _auditSerializer;

        public AuditingInterceptor(
            IAuditingHelper auditingHelper,
            IAuditingConfiguration auditingConfiguration,
            IAuditSerializer auditSerializer)
        {
            _auditingHelper = auditingHelper;
            _auditingConfiguration = auditingConfiguration;
            _auditSerializer = auditSerializer;
        }

        public override void InterceptSynchronous(IInvocation invocation)
        {
            if (KontecgCrossCuttingConcerns.IsApplied(invocation.InvocationTarget, KontecgCrossCuttingConcerns.Auditing)
               )
            {
                invocation.Proceed();
                return;
            }

            if (!_auditingHelper.ShouldSaveAudit(invocation.MethodInvocationTarget))
            {
                invocation.Proceed();
                return;
            }

            AuditInfo auditInfo = _auditingHelper.CreateAuditInfo(invocation.TargetType,
                invocation.MethodInvocationTarget,
                invocation.Arguments);

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                auditInfo.Exception = ex;
                throw;
            }
            finally
            {
                stopwatch.Stop();
                auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);

                if (_auditingConfiguration.SaveReturnValues && invocation.ReturnValue != null)
                {
                    auditInfo.ReturnValue = _auditSerializer.Serialize(invocation.ReturnValue);
                }

                _auditingHelper.Save(auditInfo);
            }
        }

        protected override async Task InternalInterceptAsynchronousAsync(IInvocation invocation)
        {
            IInvocationProceedInfo proceedInfo = invocation.CaptureProceedInfo();

            if (KontecgCrossCuttingConcerns.IsApplied(invocation.InvocationTarget, KontecgCrossCuttingConcerns.Auditing)
               )
            {
                proceedInfo.Invoke();
                Task task = (Task) invocation.ReturnValue;
                await task;
                return;
            }

            if (!_auditingHelper.ShouldSaveAudit(invocation.MethodInvocationTarget))
            {
                proceedInfo.Invoke();
                Task task = (Task) invocation.ReturnValue;
                await task;
                return;
            }

            AuditInfo auditInfo = _auditingHelper.CreateAuditInfo(invocation.TargetType,
                invocation.MethodInvocationTarget,
                invocation.Arguments);

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                proceedInfo.Invoke();
                Task task = (Task) invocation.ReturnValue;
                await task;
            }
            catch (Exception ex)
            {
                auditInfo.Exception = ex;
                throw;
            }
            finally
            {
                stopwatch.Stop();
                auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);

                await _auditingHelper.SaveAsync(auditInfo);
            }
        }

        protected override async Task<TResult> InternalInterceptAsynchronousAsync<TResult>(IInvocation invocation)
        {
            IInvocationProceedInfo proceedInfo = invocation.CaptureProceedInfo();

            if (KontecgCrossCuttingConcerns.IsApplied(invocation.InvocationTarget, KontecgCrossCuttingConcerns.Auditing)
               )
            {
                proceedInfo.Invoke();
                Task<TResult> taskResult = (Task<TResult>) invocation.ReturnValue;
                return await taskResult;
            }

            if (!_auditingHelper.ShouldSaveAudit(invocation.MethodInvocationTarget))
            {
                proceedInfo.Invoke();
                Task<TResult> taskResult = (Task<TResult>) invocation.ReturnValue;
                return await taskResult;
            }

            AuditInfo auditInfo = _auditingHelper.CreateAuditInfo(invocation.TargetType,
                invocation.MethodInvocationTarget,
                invocation.Arguments);

            Stopwatch stopwatch = Stopwatch.StartNew();
            TResult result;

            try
            {
                proceedInfo.Invoke();
                Task<TResult> taskResult = (Task<TResult>) invocation.ReturnValue;
                result = await taskResult;

                if (_auditingConfiguration.SaveReturnValues && result != null)
                {
                    auditInfo.ReturnValue = _auditSerializer.Serialize(result);
                }
            }
            catch (Exception ex)
            {
                auditInfo.Exception = ex;
                throw;
            }
            finally
            {
                stopwatch.Stop();
                auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);

                await _auditingHelper.SaveAsync(auditInfo);
            }

            return result;
        }
    }
}
