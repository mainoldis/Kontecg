using System.Globalization;
using System.Threading.Tasks;
using Kontecg.Application.Clients;
using Kontecg.Authorization.Users;
using Kontecg.Baseline;
using Kontecg.Collections.Extensions;
using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Events.Bus.Entities;
using Kontecg.Events.Bus.Handlers;
using Kontecg.Localization;
using Kontecg.MultiCompany;
using Kontecg.Runtime.Caching;
using Kontecg.UI;

namespace Kontecg.Application.Features
{
    /// <summary>
    ///     Implements <see cref="IFeatureValueStore" />.
    /// </summary>
    public class KontecgFeatureValueStore<TCompany, TUser> :
        IKontecgFeatureValueStore,
        ITransientDependency,
        IEventHandler<EntityChangingEventData<ClientInfo>>,
        IEventHandler<EntityChangingEventData<ClientFeatureSetting>>,
        IEventHandler<EntityChangingEventData<CompanyFeatureSetting>>
        where TCompany : KontecgCompany<TUser>
        where TUser : KontecgUserBase
    {
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<CompanyFeatureSetting, long> _companyFeatureRepository;
        private readonly IRepository<ClientFeatureSetting, long> _clientFeatureRepository;
        private readonly IRepository<ClientInfo, string> _clientRepository;
        private readonly IFeatureManager _featureManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="KontecgFeatureValueStore{TCompany, TUser}" /> class.
        /// </summary>
        public KontecgFeatureValueStore(
            ICacheManager cacheManager,
            IRepository<CompanyFeatureSetting, long> companyFeatureRepository,
            IRepository<ClientFeatureSetting, long> clientFeatureRepository,
            IRepository<ClientInfo, string> clientRepository,
            IFeatureManager featureManager,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _cacheManager = cacheManager;
            _companyFeatureRepository = companyFeatureRepository;
            _clientFeatureRepository = clientFeatureRepository;
            _clientRepository = clientRepository;
            _featureManager = featureManager;
            _unitOfWorkManager = unitOfWorkManager;

            LocalizationManager = NullLocalizationManager.Instance;
            LocalizationSourceName = KontecgBaselineConsts.LocalizationSourceName;
        }

        public ILocalizationManager LocalizationManager { get; set; }
        protected string LocalizationSourceName { get; set; }

        /// <inheritdoc/>
        public virtual Task<string> GetValueOrNullAsync(int companyId, Feature feature)
        {
            return GetValueOrNullAsync(companyId, feature.Name);
        }

        /// <inheritdoc/>
        public virtual string GetValueOrNull(int companyId, Feature feature)
        {
            return GetValueOrNull(companyId, feature.Name);
        }

        public virtual async Task<string> GetClientValueOrNullAsync(string clientId, string featureName)
        {
            var cacheItem = await GetClientFeatureCacheItemAsync(clientId);
            return cacheItem.FeatureValues.GetOrDefault(featureName);
        }

        public virtual string GetClientValueOrNull(string clientId, string featureName)
        {
            var cacheItem = GetClientFeatureCacheItem(clientId);
            return cacheItem.FeatureValues.GetOrDefault(featureName);
        }

        public virtual async Task<string> GetValueOrNullAsync(int companyId, string featureName)
        {
            var cacheItem = await GetCompanyFeatureCacheItemAsync(companyId);
            var value = cacheItem.FeatureValues.GetOrDefault(featureName);
            return value;
        }

        public virtual string GetValueOrNull(int companyId, string featureName)
        {
            var cacheItem = GetCompanyFeatureCacheItem(companyId);
            var value = cacheItem.FeatureValues.GetOrDefault(featureName);
            return value;
        }

        public virtual async Task SetClientFeatureValueAsync(string clientId, string featureName, string value)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                ClientInfo client;
                using (var uow = _unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.SetCompanyId(null))
                    {
                        client = await _clientRepository.GetAsync(clientId);
                        await uow.CompleteAsync();
                    }
                }

                using (_unitOfWorkManager.Current.EnableFilter(KontecgDataFilters.MustHaveCompany))
                using (_unitOfWorkManager.Current.SetCompanyId(client.CompanyId))
                {
                    if (await GetClientValueOrNullAsync(clientId, featureName) == value)
                    {
                        return;
                    }

                    var currentFeature = await _clientFeatureRepository.FirstOrDefaultAsync(f => f.ClientId == clientId && f.Name == featureName);

                    var feature = _featureManager.GetOrNull(featureName);
                    if (feature == null || feature.DefaultValue == value)
                    {
                        if (currentFeature != null)
                        {
                            await _clientFeatureRepository.DeleteAsync(currentFeature);
                        }

                        return;
                    }

                    if (!feature.InputType.Validator.IsValid(value))
                    {
                        throw new UserFriendlyException(string.Format(
                            L("InvalidFeatureValue"), feature.Name));
                    }

                    if (currentFeature == null)
                    {
                        await _clientFeatureRepository.InsertAsync(new ClientFeatureSetting(clientId, featureName, value));
                    }
                    else
                    {
                        currentFeature.Value = value;
                    }
                }
            });
        }

        public virtual void SetClientFeatureValue(string clientId, string featureName, string value)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                ClientInfo client;
                using (var uow = _unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.SetCompanyId(null))
                    {
                        client = _clientRepository.Get(clientId);
                        uow.Complete();
                    }
                }

                using (_unitOfWorkManager.Current.EnableFilter(KontecgDataFilters.MustHaveCompany))
                using (_unitOfWorkManager.Current.SetCompanyId(client.CompanyId))
                {
                    if (GetClientValueOrNull(clientId, featureName) == value)
                    {
                        return;
                    }

                    var currentFeature = _clientFeatureRepository.FirstOrDefault(f => f.ClientId == clientId && f.Name == featureName);

                    var feature = _featureManager.GetOrNull(featureName);
                    if (feature == null || feature.DefaultValue == value)
                    {
                        if (currentFeature != null)
                        {
                            _clientFeatureRepository.Delete(currentFeature);
                        }

                        return;
                    }

                    if (currentFeature == null)
                    {
                        _clientFeatureRepository.Insert(new ClientFeatureSetting(clientId, featureName, value));
                    }
                    else
                    {
                        currentFeature.Value = value;
                    }
                }
            });
        }

        protected virtual async Task<CompanyFeatureCacheItem> GetCompanyFeatureCacheItemAsync(int companyId)
        {
            return await _cacheManager.GetCompanyFeatureCache().GetAsync(companyId, async () =>
            {
                
                var newCacheItem = new CompanyFeatureCacheItem();
                using (var uow = _unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.EnableFilter(KontecgDataFilters.MayHaveCompany))
                    using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                    {
                        var featureSettings = await _companyFeatureRepository.GetAllListAsync();
                        foreach (var featureSetting in featureSettings)
                        {
                            newCacheItem.FeatureValues[featureSetting.Name] = featureSetting.Value;
                        }

                        await uow.CompleteAsync();
                    }
                }

                return newCacheItem;
            });
        }

        protected virtual CompanyFeatureCacheItem GetCompanyFeatureCacheItem(int companyId)
        {
            return _cacheManager.GetCompanyFeatureCache().Get(companyId, () =>
            {
                var newCacheItem = new CompanyFeatureCacheItem();
                using (var uow = _unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.EnableFilter(KontecgDataFilters.MayHaveCompany))
                    using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                    {
                        var featureSettings = _companyFeatureRepository.GetAllList();
                        foreach (var featureSetting in featureSettings)
                        {
                            newCacheItem.FeatureValues[featureSetting.Name] = featureSetting.Value;
                        }

                        uow.Complete();
                    }
                }

                return newCacheItem;
            });
        }

        protected virtual async Task<ClientFeatureCacheItem> GetClientFeatureCacheItemAsync(string clientId)
        {
            return await _cacheManager
                .GetClientFeatureCache()
                .GetAsync(
                    clientId,
                    async () => await CreateClientFeatureCacheItemAsync(clientId)
                );
        }

        protected virtual ClientFeatureCacheItem GetClientFeatureCacheItem(string clientId)
        {
            return _cacheManager
                .GetClientFeatureCache()
                .Get(
                    clientId,
                    () => CreateClientFeatureCacheItem(clientId)
                );
        }

        protected virtual async Task<ClientFeatureCacheItem> CreateClientFeatureCacheItemAsync(string clientId)
        {
            ClientInfo client;
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetCompanyId(null))
                {
                    client = await _clientRepository.GetAsync(clientId);
                    await uow.CompleteAsync();
                }
            }

            var newCacheItem = new ClientFeatureCacheItem();

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.EnableFilter(KontecgDataFilters.MustHaveCompany))
                using (_unitOfWorkManager.Current.SetCompanyId(client.CompanyId))
                {
                    var featureSettings = await _clientFeatureRepository.GetAllListAsync(f => f.ClientId == clientId);
                    foreach (var featureSetting in featureSettings)
                    {
                        newCacheItem.FeatureValues[featureSetting.Name] = featureSetting.Value;
                    }

                    await uow.CompleteAsync();
                }
            }

            return newCacheItem;
        }

        protected virtual ClientFeatureCacheItem CreateClientFeatureCacheItem(string clientId)
        {
            ClientInfo client;
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetCompanyId(null))
                {
                    client = _clientRepository.Get(clientId);
                    uow.Complete();
                }
            }

            var newCacheItem = new ClientFeatureCacheItem();

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.EnableFilter(KontecgDataFilters.MustHaveCompany))
                using (_unitOfWorkManager.Current.SetCompanyId(client.CompanyId))
                {
                    var featureSettings = _clientFeatureRepository.GetAllList(f => f.ClientId == clientId);
                    foreach (var featureSetting in featureSettings)
                    {
                        newCacheItem.FeatureValues[featureSetting.Name] = featureSetting.Value;
                    }

                    uow.Complete();
                }
            }

            return newCacheItem;
        }

        public virtual void HandleEvent(EntityChangingEventData<ClientFeatureSetting> eventData)
        {
            _cacheManager.GetClientFeatureCache().Remove(eventData.Entity.ClientId);
        }

        public virtual void HandleEvent(EntityChangingEventData<ClientInfo> eventData)
        {
            if (eventData.Entity.IsTransient())
            {
                return;
            }

            _cacheManager.GetClientFeatureCache().Remove(eventData.Entity.Id);
        }

        public virtual void HandleEvent(EntityChangingEventData<CompanyFeatureSetting> eventData)
        {
            if (eventData.Entity.CompanyId.HasValue)
            {
                _cacheManager.GetCompanyFeatureCache().Remove(eventData.Entity.CompanyId.Value);
            }
        }

        protected virtual string L(string name)
        {
            return LocalizationManager.GetString(LocalizationSourceName, name);
        }

        protected virtual string L(string name, CultureInfo cultureInfo)
        {
            return LocalizationManager.GetString(LocalizationSourceName, name, cultureInfo);
        }
    }
}
