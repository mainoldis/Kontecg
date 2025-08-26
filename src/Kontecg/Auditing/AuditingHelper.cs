using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using Castle.Core.Logging;
using Kontecg.Collections.Extensions;
using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Kontecg.Runtime.Session;
using Kontecg.Timing;

namespace Kontecg.Auditing
{
    public class AuditingHelper : IAuditingHelper, ITransientDependency
    {
        private readonly IAuditInfoProvider _auditInfoProvider;
        private readonly IAuditSerializer _auditSerializer;
        private readonly IAuditingConfiguration _configuration;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public AuditingHelper(
            IAuditInfoProvider auditInfoProvider,
            IAuditingConfiguration configuration,
            IUnitOfWorkManager unitOfWorkManager,
            IAuditSerializer auditSerializer)
        {
            _auditInfoProvider = auditInfoProvider;
            _configuration = configuration;
            _unitOfWorkManager = unitOfWorkManager;
            _auditSerializer = auditSerializer;

            KontecgSession = NullKontecgSession.Instance;
            Logger = NullLogger.Instance;
            AuditingStore = SimpleLogAuditingStore.Instance;
        }

        public ILogger Logger { get; set; }

        public IKontecgSession KontecgSession { get; set; }

        public IAuditingStore AuditingStore { get; set; }

        public bool ShouldSaveAudit(MethodInfo methodInfo, bool defaultValue = false)
        {
            if (!_configuration.IsEnabled)
            {
                return false;
            }

            if (!_configuration.IsEnabledForAnonymousUsers && KontecgSession?.UserId == null)
            {
                return false;
            }

            if (methodInfo == null)
            {
                return false;
            }

            if (!methodInfo.IsPublic)
            {
                return false;
            }

            if (methodInfo.IsDefined(typeof(AuditedAttribute), true))
            {
                return true;
            }

            if (methodInfo.IsDefined(typeof(DisableAuditingAttribute), true))
            {
                return false;
            }

            Type classType = methodInfo.DeclaringType;
            if (classType != null)
            {
                if (classType.GetTypeInfo().IsDefined(typeof(AuditedAttribute), true))
                {
                    return true;
                }

                if (classType.GetTypeInfo().IsDefined(typeof(DisableAuditingAttribute), true))
                {
                    return false;
                }

                if (_configuration.Selectors.Any(selector => selector.Predicate(classType)))
                {
                    return true;
                }
            }

            return defaultValue;
        }

        public AuditInfo CreateAuditInfo(Type type, MethodInfo method, object[] arguments)
        {
            return CreateAuditInfo(type, method, CreateArgumentsDictionary(method, arguments));
        }

        public AuditInfo CreateAuditInfo(Type type, MethodInfo method, IDictionary<string, object> arguments)
        {
            AuditInfo auditInfo = new AuditInfo
            {
                CompanyId = KontecgSession.CompanyId,
                UserId = KontecgSession.UserId,
                ImpersonatorUserId = KontecgSession.ImpersonatorUserId,
                ImpersonatorCompanyId = KontecgSession.ImpersonatorCompanyId,
                ServiceName = type != null
                    ? type.FullName
                    : "",
                MethodName = method.Name,
                Parameters = ConvertArgumentsToJson(arguments),
                ExecutionTime = Clock.Now
            };

            try
            {
                _auditInfoProvider.Fill(auditInfo);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString(), ex);
            }

            return auditInfo;
        }

        public void Save(AuditInfo auditInfo)
        {
            using IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress);
            AuditingStore.Save(auditInfo);
            uow.Complete();
        }

        public async Task SaveAsync(AuditInfo auditInfo)
        {
            using IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress);
            await AuditingStore.SaveAsync(auditInfo);
            await uow.CompleteAsync();
        }

        private string ConvertArgumentsToJson(IDictionary<string, object> arguments)
        {
            try
            {
                if (arguments.IsNullOrEmpty())
                {
                    return "{}";
                }

                Dictionary<string, object> dictionary = new Dictionary<string, object>();

                foreach (KeyValuePair<string, object> argument in arguments)
                {
                    if (argument.Value != null &&
                        _configuration.IgnoredTypes.Any(t => t.IsInstanceOfType(argument.Value)))
                    {
                        dictionary[argument.Key] = null;
                    }
                    else
                    {
                        dictionary[argument.Key] = argument.Value;
                    }
                }

                return _auditSerializer.Serialize(dictionary);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString(), ex);
                return "{}";
            }
        }

        private static Dictionary<string, object> CreateArgumentsDictionary(MethodInfo method, object[] arguments)
        {
            ParameterInfo[] parameters = method.GetParameters();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            for (int i = 0; i < parameters.Length; i++)
            {
                dictionary[parameters[i].Name] = arguments[i];
            }

            return dictionary;
        }
    }
}
