using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Kontecg.Application.Features;
using Kontecg.Dependency;

namespace Kontecg.Webhooks
{
    internal class WebhookDefinitionManager : IWebhookDefinitionManager, ISingletonDependency
    {
        private readonly IocManager _iocManager;
        private readonly Dictionary<string, WebhookDefinition> _webhookDefinitions;
        private readonly IWebhooksConfiguration _webhooksConfiguration;

        public WebhookDefinitionManager(
            IWebhooksConfiguration webhooksConfiguration,
            IocManager iocManager
        )
        {
            _webhooksConfiguration = webhooksConfiguration;
            _iocManager = iocManager;
            _webhookDefinitions = new Dictionary<string, WebhookDefinition>();
        }

        public void Add(WebhookDefinition webhookDefinition)
        {
            if (_webhookDefinitions.ContainsKey(webhookDefinition.Name))
            {
                throw new KontecgInitializationException("There is already a webhook definition with given name: " +
                                                         webhookDefinition.Name + ". Webhook names must be unique!");
            }

            _webhookDefinitions.Add(webhookDefinition.Name, webhookDefinition);
        }

        public WebhookDefinition GetOrNull(string name)
        {
            if (!_webhookDefinitions.ContainsKey(name))
            {
                return null;
            }

            return _webhookDefinitions[name];
        }

        public WebhookDefinition Get(string name)
        {
            if (!_webhookDefinitions.ContainsKey(name))
            {
                throw new KeyNotFoundException(
                    $"Webhook definitions does not contain a definition with the key \"{name}\".");
            }

            return _webhookDefinitions[name];
        }

        public IReadOnlyList<WebhookDefinition> GetAll()
        {
            return _webhookDefinitions.Values.ToImmutableList();
        }

        public bool Remove(string name)
        {
            return _webhookDefinitions.Remove(name);
        }

        public bool Contains(string name)
        {
            return _webhookDefinitions.ContainsKey(name);
        }

        public async Task<bool> IsAvailableAsync(int? companyId, string name)
        {
            if (companyId == null) // host allowed to subscribe all webhooks
            {
                return true;
            }

            WebhookDefinition webhookDefinition = GetOrNull(name);

            if (webhookDefinition == null)
            {
                return false;
            }

            if (webhookDefinition.FeatureDependency == null)
            {
                return true;
            }

            using IDisposableDependencyObjectWrapper<FeatureDependencyContext> featureDependencyContext =
                _iocManager.ResolveAsDisposable<FeatureDependencyContext>();
            featureDependencyContext.Object.CompanyId = companyId;

            if (!await webhookDefinition.FeatureDependency.IsSatisfiedAsync(featureDependencyContext.Object))
            {
                return false;
            }

            return true;
        }

        public bool IsAvailable(int? companyId, string name)
        {
            if (companyId == null) // host allowed to subscribe all webhooks
            {
                return true;
            }

            WebhookDefinition webhookDefinition = GetOrNull(name);

            if (webhookDefinition == null)
            {
                return false;
            }

            if (webhookDefinition.FeatureDependency == null)
            {
                return true;
            }

            using IDisposableDependencyObjectWrapper<FeatureDependencyContext> featureDependencyContext =
                _iocManager.ResolveAsDisposable<FeatureDependencyContext>();
            featureDependencyContext.Object.CompanyId = companyId;

            if (!webhookDefinition.FeatureDependency.IsSatisfied(featureDependencyContext.Object))
            {
                return false;
            }

            return true;
        }

        public void Initialize()
        {
            WebhookDefinitionContext context = new WebhookDefinitionContext(this);

            foreach (Type providerType in _webhooksConfiguration.Providers)
            {
                using IDisposableDependencyObjectWrapper<WebhookDefinitionProvider> provider =
                    _iocManager.ResolveAsDisposable<WebhookDefinitionProvider>(providerType);
                provider.Object.SetWebhooks(context);
            }
        }
    }
}
