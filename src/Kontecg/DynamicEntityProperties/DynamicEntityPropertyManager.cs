using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Kontecg.Runtime.Caching;
using Kontecg.Runtime.Session;

namespace Kontecg.DynamicEntityProperties
{
    public class DynamicEntityPropertyManager : IDynamicEntityPropertyManager, ITransientDependency
    {
        public const string CacheName = "KontecgChenetDynamicEntityPropertyCache";
        private readonly ICacheManager _cacheManager;
        private readonly IDynamicEntityPropertyDefinitionManager _dynamicEntityPropertyDefinitionManager;
        private readonly IDynamicPropertyPermissionChecker _dynamicPropertyPermissionChecker;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public DynamicEntityPropertyManager(
            IDynamicPropertyPermissionChecker dynamicPropertyPermissionChecker,
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager,
            IDynamicEntityPropertyDefinitionManager dynamicEntityPropertyDefinitionManager
        )
        {
            _dynamicPropertyPermissionChecker = dynamicPropertyPermissionChecker;
            _cacheManager = cacheManager;
            _unitOfWorkManager = unitOfWorkManager;
            _dynamicEntityPropertyDefinitionManager = dynamicEntityPropertyDefinitionManager;

            DynamicEntityPropertyStore = NullDynamicEntityPropertyStore.Instance;
            KontecgSession = NullKontecgSession.Instance;
        }

        public IDynamicEntityPropertyStore DynamicEntityPropertyStore { get; set; }
        public IKontecgSession KontecgSession { get; set; }

        private ITypedCache<string, DynamicEntityProperty> DynamicEntityPropertyCache =>
            _cacheManager.GetCache<string, DynamicEntityProperty>(CacheName);

        public virtual DynamicEntityProperty Get(int id)
        {
            int? companyId = GetCurrentCompanyId();
            string cacheKey = GetCacheKey(id, companyId);

            DynamicEntityProperty entityProperty =
                DynamicEntityPropertyCache.Get(cacheKey, () => DynamicEntityPropertyStore.Get(id));
            _dynamicPropertyPermissionChecker.CheckPermission(entityProperty.DynamicPropertyId);
            return entityProperty;
        }

        public virtual async Task<DynamicEntityProperty> GetAsync(int id)
        {
            int? companyId = GetCurrentCompanyId();
            string cacheKey = GetCacheKey(id, companyId);

            DynamicEntityProperty entityProperty =
                await DynamicEntityPropertyCache.GetAsync(cacheKey, () => DynamicEntityPropertyStore.GetAsync(id));
            await _dynamicPropertyPermissionChecker.CheckPermissionAsync(entityProperty.DynamicPropertyId);
            return entityProperty;
        }

        public List<DynamicEntityProperty> GetAll(string entityFullName)
        {
            List<DynamicEntityProperty> allProperties = DynamicEntityPropertyStore.GetAll(entityFullName);
            allProperties = allProperties.Where(dynamicEntityProperty =>
                    _dynamicPropertyPermissionChecker.IsGranted(dynamicEntityProperty.DynamicPropertyId))
                .ToList();
            return allProperties;
        }

        public async Task<List<DynamicEntityProperty>> GetAllAsync(string entityFullName)
        {
            List<DynamicEntityProperty> allProperties = await DynamicEntityPropertyStore.GetAllAsync(entityFullName);

            List<DynamicEntityProperty> controlledProperties = new List<DynamicEntityProperty>();
            foreach (DynamicEntityProperty dynamicEntityProperty in allProperties)
            {
                if (await _dynamicPropertyPermissionChecker.IsGrantedAsync(dynamicEntityProperty.DynamicPropertyId))
                {
                    controlledProperties.Add(dynamicEntityProperty);
                }
            }

            return controlledProperties;
        }

        public List<DynamicEntityProperty> GetAll()
        {
            List<DynamicEntityProperty> allProperties = DynamicEntityPropertyStore.GetAll();
            allProperties = allProperties.Where(dynamicEntityProperty =>
                    _dynamicPropertyPermissionChecker.IsGranted(dynamicEntityProperty.DynamicPropertyId))
                .ToList();
            return allProperties;
        }

        public async Task<List<DynamicEntityProperty>> GetAllAsync()
        {
            List<DynamicEntityProperty> allProperties = await DynamicEntityPropertyStore.GetAllAsync();

            List<DynamicEntityProperty> controlledProperties = new List<DynamicEntityProperty>();
            foreach (DynamicEntityProperty dynamicEntityProperty in allProperties)
            {
                if (await _dynamicPropertyPermissionChecker.IsGrantedAsync(dynamicEntityProperty.DynamicPropertyId))
                {
                    controlledProperties.Add(dynamicEntityProperty);
                }
            }

            return controlledProperties;
        }

        public virtual void Add(DynamicEntityProperty dynamicEntityProperty)
        {
            CheckEntityName(dynamicEntityProperty.EntityFullName);
            _dynamicPropertyPermissionChecker.CheckPermission(dynamicEntityProperty.DynamicPropertyId);

            using (IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                DynamicEntityPropertyStore.Add(dynamicEntityProperty);
                uow.Complete();
            }

            string cacheKey = GetCacheKey(dynamicEntityProperty.Id, dynamicEntityProperty.CompanyId);
            DynamicEntityPropertyCache.Set(cacheKey, dynamicEntityProperty);
        }

        public virtual async Task AddAsync(DynamicEntityProperty dynamicEntityProperty)
        {
            CheckEntityName(dynamicEntityProperty.EntityFullName);
            await _dynamicPropertyPermissionChecker.CheckPermissionAsync(dynamicEntityProperty.DynamicPropertyId);

            using (IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                await DynamicEntityPropertyStore.AddAsync(dynamicEntityProperty);
                await uow.CompleteAsync();
            }

            string cacheKey = GetCacheKey(dynamicEntityProperty.Id, dynamicEntityProperty.CompanyId);
            await DynamicEntityPropertyCache.SetAsync(cacheKey, dynamicEntityProperty);
        }

        public virtual void Update(DynamicEntityProperty dynamicEntityProperty)
        {
            CheckEntityName(dynamicEntityProperty.EntityFullName);
            _dynamicPropertyPermissionChecker.CheckPermission(dynamicEntityProperty.DynamicPropertyId);

            using (IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                DynamicEntityPropertyStore.Update(dynamicEntityProperty);
                uow.Complete();
            }

            string cacheKey = GetCacheKey(dynamicEntityProperty.Id, dynamicEntityProperty.CompanyId);
            DynamicEntityPropertyCache.Set(cacheKey, dynamicEntityProperty);
        }

        public virtual async Task UpdateAsync(DynamicEntityProperty dynamicEntityProperty)
        {
            CheckEntityName(dynamicEntityProperty.EntityFullName);
            await _dynamicPropertyPermissionChecker.CheckPermissionAsync(dynamicEntityProperty.DynamicPropertyId);

            using (IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                await DynamicEntityPropertyStore.UpdateAsync(dynamicEntityProperty);
                await uow.CompleteAsync();
            }

            string cacheKey = GetCacheKey(dynamicEntityProperty.Id, dynamicEntityProperty.CompanyId);
            await DynamicEntityPropertyCache.SetAsync(cacheKey, dynamicEntityProperty);
        }

        public virtual void Delete(int id)
        {
            DynamicEntityProperty dynamicEntityProperty = Get(id); //Get checks permission, no need to check it again
            if (dynamicEntityProperty == null)
            {
                return;
            }

            DynamicEntityPropertyStore.Delete(dynamicEntityProperty.Id);

            int? companyId = GetCurrentCompanyId();
            string cacheKey = GetCacheKey(id, companyId);

            DynamicEntityPropertyCache.Remove(cacheKey);
        }

        public virtual async Task DeleteAsync(int id)
        {
            DynamicEntityProperty
                dynamicEntityProperty = await GetAsync(id); //Get checks permission, no need to check it again
            if (dynamicEntityProperty == null)
            {
                return;
            }

            await DynamicEntityPropertyStore.DeleteAsync(dynamicEntityProperty.Id);

            int? companyId = GetCurrentCompanyId();
            string cacheKey = GetCacheKey(id, companyId);

            await DynamicEntityPropertyCache.RemoveAsync(cacheKey);
        }

        private void CheckEntityName(string entityFullName)
        {
            if (!_dynamicEntityPropertyDefinitionManager.ContainsEntity(entityFullName))
            {
                throw new ApplicationException($"Unknown entity {entityFullName}.");
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
