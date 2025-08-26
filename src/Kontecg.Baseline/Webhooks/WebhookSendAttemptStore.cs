using Kontecg.Application.Services.Dto;
using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Net;

namespace Kontecg.Webhooks
{
    /// <summary>
    /// Implements <see cref="IWebhookSendAttemptStore"/> using repositories.
    /// </summary>
    public class WebhookSendAttemptStore : IWebhookSendAttemptStore, ITransientDependency
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        private readonly IRepository<WebhookSendAttempt, Guid> _webhookSendAttemptRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public WebhookSendAttemptStore(
            IRepository<WebhookSendAttempt, Guid> webhookSendAttemptRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _webhookSendAttemptRepository = webhookSendAttemptRepository;
            _unitOfWorkManager = unitOfWorkManager;

            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        public virtual async Task InsertAsync(WebhookSendAttempt webhookSendAttempt)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetCompanyId(webhookSendAttempt.CompanyId))
                {
                    await _webhookSendAttemptRepository.InsertAsync(webhookSendAttempt);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }

                await uow.CompleteAsync();
            }
        }

        public virtual void Insert(WebhookSendAttempt webhookSendAttempt)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetCompanyId(webhookSendAttempt.CompanyId))
                {
                    _webhookSendAttemptRepository.Insert(webhookSendAttempt);
                    _unitOfWorkManager.Current.SaveChanges();
                }

                uow.Complete();
            }
        }

        public virtual async Task UpdateAsync(WebhookSendAttempt webhookSendAttempt)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetCompanyId(webhookSendAttempt.CompanyId))
                {
                    await _webhookSendAttemptRepository.UpdateAsync(webhookSendAttempt);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }

                await uow.CompleteAsync();
            }
        }

        public virtual void Update(WebhookSendAttempt webhookSendAttempt)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetCompanyId(webhookSendAttempt.CompanyId))
                {
                    _webhookSendAttemptRepository.Update(webhookSendAttempt);
                    _unitOfWorkManager.Current.SaveChanges();
                }

                uow.Complete();
            }
        }

        public virtual async Task DeleteAsync(WebhookSendAttempt webhookSendAttempt)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetCompanyId(webhookSendAttempt.CompanyId))
                {
                    await _webhookSendAttemptRepository.DeleteAsync(webhookSendAttempt);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }

                await uow.CompleteAsync();
            }
        }

        public void Delete(WebhookSendAttempt webhookSendAttempt)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetCompanyId(webhookSendAttempt.CompanyId))
                {
                    _webhookSendAttemptRepository.Delete(webhookSendAttempt);
                    _unitOfWorkManager.Current.SaveChanges();
                }

                uow.Complete();
            }
        }

        public virtual async Task<WebhookSendAttempt> GetAsync(int? companyId, Guid id)
        {
            WebhookSendAttempt sendAttempt;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    sendAttempt = await _webhookSendAttemptRepository.GetAsync(id);
                }

                await uow.CompleteAsync();
            }

            return sendAttempt;
        }

        public virtual WebhookSendAttempt Get(int? companyId, Guid id)
        {
            WebhookSendAttempt sendAttempt;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    sendAttempt = _webhookSendAttemptRepository.Get(id);
                }

                uow.Complete();
            }

            return sendAttempt;
        }

        public virtual async Task<int> GetSendAttemptCountAsync(int? companyId, Guid webhookEventId,
            Guid webhookSubscriptionId)
        {
            int sendAttemptCount;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    sendAttemptCount = await _webhookSendAttemptRepository
                        .CountAsync(attempt =>
                            attempt.WebhookEventId == webhookEventId &&
                            attempt.WebhookSubscriptionId == webhookSubscriptionId
                        );
                }

                await uow.CompleteAsync();
            }

            return sendAttemptCount;
        }

        public virtual int GetSendAttemptCount(int? companyId, Guid webhookId, Guid webhookSubscriptionId)
        {
            int sendAttemptCount;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    sendAttemptCount = _webhookSendAttemptRepository.GetAll()
                        .Count(attempt =>
                            attempt.WebhookEventId == webhookId &&
                            attempt.WebhookSubscriptionId == webhookSubscriptionId);
                }

                uow.Complete();
            }

            return sendAttemptCount;
        }

        public virtual async Task<bool> HasXConsecutiveFailAsync(int? companyId, Guid subscriptionId, int failCount)
        {
            bool result;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    if (await _webhookSendAttemptRepository.CountAsync(x => x.WebhookSubscriptionId == subscriptionId) <
                        failCount)
                    {
                        result = false;
                    }
                    else
                    {
                        result = !await AsyncQueryableExecuter.AnyAsync(
                            (await _webhookSendAttemptRepository.GetAllAsync())
                                .OrderByDescending(attempt => attempt.CreationTime)
                                .Take(failCount)
                                .Where(attempt => attempt.ResponseStatusCode == HttpStatusCode.OK)
                        );
                    }
                }

                await uow.CompleteAsync();
            }

            return result;
        }

        public virtual async Task<IPagedResult<WebhookSendAttempt>> GetAllSendAttemptsBySubscriptionAsPagedListAsync(
            int? companyId,
            Guid subscriptionId,
            int maxResultCount,
            int skipCount)
        {
            PagedResultDto<WebhookSendAttempt> sendAttempts;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    var query = _webhookSendAttemptRepository.GetAllIncluding(attempt => attempt.WebhookEvent)
                        .Where(attempt =>
                            attempt.WebhookSubscriptionId == subscriptionId
                        );

                    var totalCount = await AsyncQueryableExecuter.CountAsync(query);

                    var list = await AsyncQueryableExecuter.ToListAsync(query
                        .OrderByDescending(attempt => attempt.CreationTime)
                        .Skip(skipCount)
                        .Take(maxResultCount)
                    );

                    sendAttempts = new PagedResultDto<WebhookSendAttempt>
                    {
                        TotalCount = totalCount,
                        Items = list
                    };
                }

                await uow.CompleteAsync();
            }

            return sendAttempts;
        }

        public virtual IPagedResult<WebhookSendAttempt> GetAllSendAttemptsBySubscriptionAsPagedList(int? companyId,
            Guid subscriptionId, int maxResultCount, int skipCount)
        {
            PagedResultDto<WebhookSendAttempt> sendAttempts;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    var query = _webhookSendAttemptRepository.GetAllIncluding(attempt => attempt.WebhookEvent)
                        .Where(attempt =>
                            attempt.WebhookSubscriptionId == subscriptionId
                        );

                    var totalCount = query.Count();

                    var list = query
                        .OrderByDescending(attempt => attempt.CreationTime)
                        .Skip(skipCount)
                        .Take(maxResultCount)
                        .ToList();

                    sendAttempts = new PagedResultDto<WebhookSendAttempt>()
                    {
                        TotalCount = totalCount,
                        Items = list
                    };
                }

                uow.Complete();
            }

            return sendAttempts;
        }

        public virtual async Task<List<WebhookSendAttempt>> GetAllSendAttemptsByWebhookEventIdAsync(int? companyId,
            Guid webhookEventId)
        {
            List<WebhookSendAttempt> sendAttempts;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    sendAttempts = await AsyncQueryableExecuter.ToListAsync(
                        (await _webhookSendAttemptRepository.GetAllAsync())
                            .Where(attempt => attempt.WebhookEventId == webhookEventId)
                            .OrderByDescending(attempt => attempt.CreationTime)
                    );
                }

                await uow.CompleteAsync();
            }

            return sendAttempts;
        }

        public virtual List<WebhookSendAttempt> GetAllSendAttemptsByWebhookEventId(int? companyId, Guid webhookEventId)
        {
            List<WebhookSendAttempt> sendAttempts;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    sendAttempts = _webhookSendAttemptRepository.GetAll()
                        .Where(attempt => attempt.WebhookEventId == webhookEventId)
                        .OrderByDescending(attempt => attempt.CreationTime).ToList();
                }

                uow.Complete();
            }

            return sendAttempts;
        }
    }
}
