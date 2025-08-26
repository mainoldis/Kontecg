using System;
using System.Threading.Tasks;
using System.Transactions;
using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Kontecg.Extensions;
using Kontecg.Runtime.Caching;
using Kontecg.Runtime.Session;

namespace Kontecg.DynamicEntityProperties
{
    public class DynamicPropertyManager : IDynamicPropertyManager, ITransientDependency
    {
        public const string CacheName = "KontecgChenetDynamicPropertyCache";
        private readonly ICacheManager _cacheManager;
        private readonly IDynamicEntityPropertyDefinitionManager _dynamicEntityPropertyDefinitionManager;
        private readonly IDynamicPropertyStore _dynamicPropertyStore;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public DynamicPropertyManager(
            ICacheManager cacheManager,
            IDynamicPropertyStore dynamicPropertyStore,
            IUnitOfWorkManager unitOfWorkManager,
            IDynamicEntityPropertyDefinitionManager dynamicEntityPropertyDefinitionManager
        )
        {
            _cacheManager = cacheManager;
            _dynamicPropertyStore = dynamicPropertyStore;
            _unitOfWorkManager = unitOfWorkManager;
            _dynamicEntityPropertyDefinitionManager = dynamicEntityPropertyDefinitionManager;

            KontecgSession = NullKontecgSession.Instance;
        }

        public IKontecgSession KontecgSession { get; set; }

        private ITypedCache<string, DynamicProperty> DynamicPropertyCache =>
            _cacheManager.GetCache<string, DynamicProperty>(CacheName);

        public virtual DynamicProperty Get(int id)
        {
            int? companyId = GetCurrentCompanyId();
            string cacheKey = GetCacheKey(id, companyId);

            return DynamicPropertyCache.Get(cacheKey, () => _dynamicPropertyStore.Get(id));
        }

        public virtual Task<DynamicProperty> GetAsync(int id)
        {
            int? companyId = GetCurrentCompanyId();
            string cacheKey = GetCacheKey(id, companyId);

            return DynamicPropertyCache.GetAsync(cacheKey, i => _dynamicPropertyStore.GetAsync(id));
        }

        public virtual DynamicProperty Get(string propertyName)
        {
            return _dynamicPropertyStore.Get(propertyName);
        }

        public virtual Task<DynamicProperty> GetAsync(string propertyName)
        {
            return _dynamicPropertyStore.GetAsync(propertyName);
        }

        public virtual DynamicProperty Add(DynamicProperty dynamicProperty)
        {
            CheckDynamicProperty(dynamicProperty);

            using (IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                _dynamicPropertyStore.Add(dynamicProperty);
                uow.Complete();
            }

            string cacheKey = GetCacheKey(dynamicProperty.Id, dynamicProperty.CompanyId);
            DynamicPropertyCache.Set(cacheKey, dynamicProperty);

            return dynamicProperty;
        }

        public virtual async Task<DynamicProperty> AddAsync(DynamicProperty dynamicProperty)
        {
            CheckDynamicProperty(dynamicProperty);

            using (IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                await _dynamicPropertyStore.AddAsync(dynamicProperty);
                await uow.CompleteAsync();
            }

            string cacheKey = GetCacheKey(dynamicProperty.Id, dynamicProperty.CompanyId);
            await DynamicPropertyCache.SetAsync(cacheKey, dynamicProperty);

            return dynamicProperty;
        }

        public virtual DynamicProperty Update(DynamicProperty dynamicProperty)
        {
            CheckDynamicProperty(dynamicProperty);

            using (IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                _dynamicPropertyStore.Update(dynamicProperty);
                uow.Complete();
            }

            string cacheKey = GetCacheKey(dynamicProperty.Id, dynamicProperty.CompanyId);
            DynamicPropertyCache.Set(cacheKey, dynamicProperty);

            return dynamicProperty;
        }

        public virtual async Task<DynamicProperty> UpdateAsync(DynamicProperty dynamicProperty)
        {
            CheckDynamicProperty(dynamicProperty);

            using (IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                await _dynamicPropertyStore.UpdateAsync(dynamicProperty);
                await uow.CompleteAsync();
            }

            string cacheKey = GetCacheKey(dynamicProperty.Id, dynamicProperty.CompanyId);
            await DynamicPropertyCache.SetAsync(cacheKey, dynamicProperty);

            return dynamicProperty;
        }

        public virtual void Delete(int id)
        {
            using (IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                _dynamicPropertyStore.Delete(id);
                uow.Complete();
            }

            int? companyId = GetCurrentCompanyId();
            string cacheKey = GetCacheKey(id, companyId);

            DynamicPropertyCache.Remove(cacheKey);
        }

        public virtual async Task DeleteAsync(int id)
        {
            using (IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                await _dynamicPropertyStore.DeleteAsync(id);
                await uow.CompleteAsync();
            }

            int? companyId = GetCurrentCompanyId();
            string cacheKey = GetCacheKey(id, companyId);

            await DynamicPropertyCache.RemoveAsync(cacheKey);
        }

        protected virtual void CheckDynamicProperty(DynamicProperty dynamicProperty)
        {
            if (dynamicProperty == null)
            {
                throw new ArgumentNullException(nameof(dynamicProperty));
            }

            if (dynamicProperty.PropertyName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(dynamicProperty.PropertyName));
            }

            if (!_dynamicEntityPropertyDefinitionManager.ContainsInputType(dynamicProperty.InputType))
            {
                throw new ApplicationException(
                    $"Input type is invalid, if you want to use \"{dynamicProperty.InputType}\" input type, define it in DynamicEntityPropertyDefinitionProvider.");
            }
        }

        protected virtual int? GetCurrentCompanyId()
        {
            if (_unitOfWorkManager.Current != null)
            {
                return _unitOfWorkManager.Current.GetCompanyId();
            }

            return KontecgSession.CompanyId;
        }

        protected virtual string GetCacheKey(int id, int? companyId)
        {
            return id + "@" + (companyId ?? 0);
        }
    }
}
