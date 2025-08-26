using System;
using System.Threading.Tasks;
using System.Transactions;
using Kontecg.BackgroundJobs;
using Kontecg.Dependency;
using Kontecg.Domain.Uow;

namespace Kontecg.Webhooks.BackgroundWorker
{
    public class WebhookSenderJob : AsyncBackgroundJob<WebhookSenderArgs>, ITransientDependency
    {
        private readonly IWebhooksConfiguration _webhooksConfiguration;
        private readonly IWebhookSendAttemptStore _webhookSendAttemptStore;
        private readonly IWebhookSender _webhookSender;
        private readonly IWebhookSubscriptionManager _webhookSubscriptionManager;

        public WebhookSenderJob(
            IWebhooksConfiguration webhooksConfiguration,
            IWebhookSubscriptionManager webhookSubscriptionManager,
            IWebhookSendAttemptStore webhookSendAttemptStore,
            IWebhookSender webhookSender)
        {
            _webhooksConfiguration = webhooksConfiguration;
            _webhookSubscriptionManager = webhookSubscriptionManager;
            _webhookSendAttemptStore = webhookSendAttemptStore;
            _webhookSender = webhookSender;
        }

        public override async Task ExecuteAsync(WebhookSenderArgs args)
        {
            if (args.TryOnce)
            {
                try
                {
                    await SendWebhookAsync(args);
                }
                catch (Exception e)
                {
                    Logger.Warn("An error occured while sending webhook with try once.", e);
                    // ignored
                }
            }
            else
            {
                await SendWebhookAsync(args);
            }
        }

        private async Task SendWebhookAsync(WebhookSenderArgs args)
        {
            if (args.WebhookEventId == default)
            {
                return;
            }

            if (args.WebhookSubscriptionId == default)
            {
                return;
            }

            if (!args.TryOnce)
            {
                int sendAttemptCount = await _webhookSendAttemptStore.GetSendAttemptCountAsync(
                    args.CompanyId,
                    args.WebhookEventId,
                    args.WebhookSubscriptionId
                );

                if (sendAttemptCount > _webhooksConfiguration.SendAttemptCount)
                {
                    return;
                }
            }

            try
            {
                await _webhookSender.SendWebhookAsync(args);
            }
            catch (Exception)
            {
                // no need to retry to send webhook since subscription disabled
                if (!await TryDeactivateSubscriptionIfReachedMaxConsecutiveFailCountAsync(
                        args.CompanyId,
                        args.WebhookSubscriptionId))
                {
                    throw; //Throw exception to re-try sending webhook
                }
            }
        }

        private async Task<bool> TryDeactivateSubscriptionIfReachedMaxConsecutiveFailCountAsync(int? companyId,
            Guid subscriptionId)
        {
            if (!_webhooksConfiguration.IsAutomaticSubscriptionDeactivationEnabled)
            {
                return false;
            }

            bool hasXConsecutiveFail = await _webhookSendAttemptStore
                .HasXConsecutiveFailAsync(
                    companyId,
                    subscriptionId,
                    _webhooksConfiguration.ConsecutiveFailCountBeforeDeactivateSubscription
                );

            if (!hasXConsecutiveFail)
            {
                return false;
            }

            using IUnitOfWorkCompleteHandle uow = UnitOfWorkManager.Begin(TransactionScopeOption.Required);
            await _webhookSubscriptionManager.ActivateWebhookSubscriptionAsync(subscriptionId, false);
            await uow.CompleteAsync();
            return true;
        }
    }
}
