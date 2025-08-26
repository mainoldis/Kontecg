using System.Threading.Tasks;
using Kontecg.Authorization.Users;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Events.Bus.Entities;
using Kontecg.Events.Bus.Handlers;
using Kontecg.Runtime.Caching;
using Kontecg.Runtime.Security;

namespace Kontecg.MultiCompany
{
    public class CompanyCache<TCompany, TUser> : ICompanyCache, IEventHandler<EntityChangedEventData<TCompany>>
        where TCompany : KontecgCompany<TUser>
        where TUser : KontecgUserBase
    {
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<TCompany> _companyRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public CompanyCache(
            ICacheManager cacheManager,
            IRepository<TCompany> companyRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _cacheManager = cacheManager;
            _companyRepository = companyRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public virtual CompanyCacheItem Get(int companyId)
        {
            CompanyCacheItem cacheItem = GetOrNull(companyId);

            if (cacheItem == null)
            {
                throw new KontecgException("There is no company with given id: " + companyId);
            }

            return cacheItem;
        }

        public virtual CompanyCacheItem Get(string companyName)
        {
            CompanyCacheItem cacheItem = GetOrNull(companyName);

            if (cacheItem == null)
            {
                throw new KontecgException("There is no company with given company name: " + companyName);
            }

            return cacheItem;
        }

        public virtual CompanyCacheItem GetOrNull(string companyName)
        {
            int? companyId = _cacheManager
                .GetCompanyByNameCache()
                .Get(
                    companyName.ToLowerInvariant(),
                    () => GetCompanyOrNull(companyName)?.Id
                );

            if (companyId == null)
            {
                return null;
            }

            return Get(companyId.Value);
        }

        public CompanyCacheItem GetOrNull(int companyId)
        {
            return _cacheManager
                .GetCompanyCache()
                .Get(
                    companyId,
                    () =>
                    {
                        TCompany company = GetCompanyOrNull(companyId);
                        if (company == null)
                        {
                            return null;
                        }

                        return CreateCompanyCacheItem(company);
                    }
                );
        }

        public virtual async Task<CompanyCacheItem> GetAsync(int companyId)
        {
            CompanyCacheItem cacheItem = await GetOrNullAsync(companyId);

            if (cacheItem == null)
            {
                throw new KontecgException("There is no company with given id: " + companyId);
            }

            return cacheItem;
        }

        public virtual async Task<CompanyCacheItem> GetAsync(string companyName)
        {
            CompanyCacheItem cacheItem = await GetOrNullAsync(companyName);

            if (cacheItem == null)
            {
                throw new KontecgException("There is no company with given company name: " + companyName);
            }

            return cacheItem;
        }

        public virtual async Task<CompanyCacheItem> GetOrNullAsync(string companyName)
        {
            int? companyId = await _cacheManager
                .GetCompanyByNameCache()
                .GetAsync(
                    companyName.ToLowerInvariant(), async key => (await GetCompanyOrNullAsync(companyName))?.Id
                );

            if (companyId == null)
            {
                return null;
            }

            return await GetAsync(companyId.Value);
        }

        public virtual async Task<CompanyCacheItem> GetOrNullAsync(int companyId)
        {
            return await _cacheManager
                .GetCompanyCache()
                .GetAsync(
                    companyId, async key =>
                    {
                        TCompany company = await GetCompanyOrNullAsync(companyId);
                        if (company == null)
                        {
                            return null;
                        }

                        return CreateCompanyCacheItem(company);
                    }
                );
        }

        public virtual void HandleEvent(EntityChangedEventData<TCompany> eventData)
        {
            CompanyCacheItem existingCacheItem = _cacheManager.GetCompanyCache().GetOrDefault(eventData.Entity.Id);

            _cacheManager
                .GetCompanyByNameCache()
                .Remove(
                    existingCacheItem != null
                        ? existingCacheItem.CompanyName.ToLowerInvariant()
                        : eventData.Entity.CompanyName.ToLowerInvariant()
                );

            _cacheManager
                .GetCompanyCache()
                .Remove(eventData.Entity.Id);
        }

        protected virtual CompanyCacheItem CreateCompanyCacheItem(TCompany company)
        {
            return new CompanyCacheItem
            {
                Id = company.Id,
                Name = company.Name,
                CompanyName = company.CompanyName,
                ConnectionString = SimpleStringCipher.Instance.Decrypt(company.ConnectionString),
                IsActive = company.IsActive
            };
        }

        protected virtual TCompany GetCompanyOrNull(int companyId)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(null))
                {
                    return _companyRepository.FirstOrDefault(companyId);
                }
            });
        }

        protected virtual TCompany GetCompanyOrNull(string companyName)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(null))
                {
                    return _companyRepository.FirstOrDefault(t => t.CompanyName == companyName);
                }
            });
        }

        protected virtual async Task<TCompany> GetCompanyOrNullAsync(int companyId)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(null))
                {
                    return await _companyRepository.FirstOrDefaultAsync(companyId);
                }
            });
        }

        protected virtual async Task<TCompany> GetCompanyOrNullAsync(string companyName)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(null))
                {
                    return await _companyRepository.FirstOrDefaultAsync(t => t.CompanyName == companyName);
                }
            });
        }
    }
}
