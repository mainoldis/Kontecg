using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Kontecg.Application.Features;
using Kontecg.Authorization.Users;
using Kontecg.Baseline;
using Kontecg.Collections.Extensions;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Services;
using Kontecg.Domain.Uow;
using Kontecg.Events.Bus.Entities;
using Kontecg.Events.Bus.Handlers;
using Kontecg.Localization;
using Kontecg.Runtime.Caching;
using Kontecg.UI;

namespace Kontecg.MultiCompany
{
    /// <summary>
    ///     Company manager.
    ///     Implements domain logic for <see cref="KontecgCompany{TUser}" />.
    /// </summary>
    /// <typeparam name="TCompany">Type of the application Company</typeparam>
    /// <typeparam name="TUser">Type of the application User</typeparam>
    public class KontecgCompanyManager<TCompany, TUser> : IDomainService,
        IEventHandler<EntityChangedEventData<TCompany>>
        where TCompany : KontecgCompany<TUser>
        where TUser : KontecgUserBase
    {
        private readonly IKontecgFeatureValueStore _featureValueStore;

        public KontecgCompanyManager(
            IRepository<TCompany> companyRepository,
            IRepository<CompanyFeatureSetting, long> companyFeatureRepository,
            IKontecgFeatureValueStore featureValueStore)
        {
            _featureValueStore = featureValueStore;
            CompanyRepository = companyRepository;
            CompanyFeatureRepository = companyFeatureRepository;
            LocalizationManager = NullLocalizationManager.Instance;
            LocalizationSourceName = KontecgBaselineConsts.LocalizationSourceName;
        }

        public ILocalizationManager LocalizationManager { get; set; }

        public ICacheManager CacheManager { get; set; }

        public IFeatureManager FeatureManager { get; set; }

        public IUnitOfWorkManager UnitOfWorkManager { get; set; }

        public virtual IQueryable<TCompany> Companies => CompanyRepository.GetAll();

        protected string LocalizationSourceName { get; set; }

        protected IRepository<TCompany> CompanyRepository { get; set; }

        protected IRepository<CompanyFeatureSetting, long> CompanyFeatureRepository { get; set; }

        public void HandleEvent(EntityChangedEventData<TCompany> eventData)
        {
            if (eventData.Entity.IsTransient())
            {
                return;
            }

            CacheManager.GetCompanyFeatureCache().Remove(eventData.Entity.Id);
        }

        public virtual async Task CreateAsync(TCompany company)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await ValidateCompanyAsync(company);

                if (await CompanyRepository.FirstOrDefaultAsync(t => t.CompanyName == company.CompanyName) != null)
                {
                    throw new UserFriendlyException(string.Format(L("CompanyNameIsAlreadyTaken"), company.CompanyName));
                }

                await CompanyRepository.InsertAsync(company);
            });
        }

        public virtual void Create(TCompany company)
        {
            UnitOfWorkManager.WithUnitOfWork(() =>
            {
                ValidateCompany(company);

                if (CompanyRepository.FirstOrDefault(t => t.CompanyName == company.CompanyName) != null)
                {
                    throw new UserFriendlyException(string.Format(L("CompanyNameIsAlreadyTaken"), company.CompanyName));
                }

                CompanyRepository.Insert(company);
            });
        }

        public virtual async Task UpdateAsync(TCompany company)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                if (await CompanyRepository.FirstOrDefaultAsync(t =>
                        t.CompanyName == company.CompanyName && t.Id != company.Id) != null)
                {
                    throw new UserFriendlyException(string.Format(L("CompanyNameIsAlreadyTaken"), company.CompanyName));
                }

                await CompanyRepository.UpdateAsync(company);
            });
        }

        public virtual void Update(TCompany company)
        {
            UnitOfWorkManager.WithUnitOfWork(() =>
            {
                if (CompanyRepository.FirstOrDefault(t => t.CompanyName == company.CompanyName && t.Id != company.Id) !=
                    null)
                {
                    throw new UserFriendlyException(string.Format(L("CompanyNameIsAlreadyTaken"), company.CompanyName));
                }

                CompanyRepository.Update(company);
            });
        }

        public virtual async Task<TCompany> FindByIdAsync(int id)
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await CompanyRepository.FirstOrDefaultAsync(id));
        }

        public virtual TCompany FindById(int id)
        {
            return UnitOfWorkManager.WithUnitOfWork(() => CompanyRepository.FirstOrDefault(id));
        }

        public virtual async Task<TCompany> GetByIdAsync(int id)
        {
            TCompany company = await FindByIdAsync(id);
            if (company == null)
            {
                throw new KontecgException("There is no company with id: " + id);
            }

            return company;
        }

        public virtual TCompany GetById(int id)
        {
            TCompany company = FindById(id);
            if (company == null)
            {
                throw new KontecgException("There is no company with id: " + id);
            }

            return company;
        }

        public virtual async Task<TCompany> FindByCompanyNameAsync(string companyName)
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await CompanyRepository.FirstOrDefaultAsync(t => t.CompanyName == companyName);
            });
        }

        public virtual TCompany FindByCompanyName(string companyName)
        {
            return UnitOfWorkManager.WithUnitOfWork(() =>
            {
                return CompanyRepository.FirstOrDefault(t => t.CompanyName == companyName);
            });
        }

        public virtual async Task DeleteAsync(TCompany company)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () => { await CompanyRepository.DeleteAsync(company); });
        }

        public virtual void Delete(TCompany company)
        {
            UnitOfWorkManager.WithUnitOfWork(() => { CompanyRepository.Delete(company); });
        }

        public Task<string> GetFeatureValueOrNullAsync(int companyId, string featureName)
        {
            return _featureValueStore.GetValueOrNullAsync(companyId, featureName);
        }

        public string GetFeatureValueOrNull(int companyId, string featureName)
        {
            return _featureValueStore.GetValueOrNull(companyId, featureName);
        }

        public virtual async Task<IReadOnlyList<NameValue>> GetFeatureValuesAsync(int companyId)
        {
            List<NameValue> values = new List<NameValue>();

            foreach (Feature feature in FeatureManager.GetAll())
            {
                values.Add(new NameValue(feature.Name,
                    await GetFeatureValueOrNullAsync(companyId, feature.Name) ?? feature.DefaultValue));
            }

            return values;
        }

        public virtual IReadOnlyList<NameValue> GetFeatureValues(int companyId)
        {
            List<NameValue> values = new List<NameValue>();

            foreach (Feature feature in FeatureManager.GetAll())
            {
                values.Add(new NameValue(feature.Name,
                    GetFeatureValueOrNull(companyId, feature.Name) ?? feature.DefaultValue));
            }

            return values;
        }

        public virtual async Task SetFeatureValuesAsync(int companyId, params NameValue[] values)
        {
            if (values.IsNullOrEmpty())
            {
                return;
            }

            foreach (NameValue value in values)
            {
                await SetFeatureValueAsync(companyId, value.Name, value.Value);
            }
        }

        public virtual void SetFeatureValues(int companyId, params NameValue[] values)
        {
            if (values.IsNullOrEmpty())
            {
                return;
            }

            foreach (NameValue value in values)
            {
                SetFeatureValue(companyId, value.Name, value.Value);
            }
        }

        public virtual async Task SetFeatureValueAsync(int companyId, string featureName, string value)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await SetFeatureValueAsync(await GetByIdAsync(companyId), featureName, value);
            });
        }

        public virtual void SetFeatureValue(int companyId, string featureName, string value)
        {
            UnitOfWorkManager.WithUnitOfWork(() => { SetFeatureValue(GetById(companyId), featureName, value); });
        }

        public virtual async Task SetFeatureValueAsync(TCompany company, string featureName, string value)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                //No need to change if it's already equals to the current value
                if (await GetFeatureValueOrNullAsync(company.Id, featureName) == value)
                {
                    return;
                }

                //Get the current feature setting
                CompanyFeatureSetting currentSetting;
                using (UnitOfWorkManager.Current.EnableFilter(KontecgDataFilters.MayHaveCompany))
                using (UnitOfWorkManager.Current.SetCompanyId(company.Id))
                {
                    currentSetting = await CompanyFeatureRepository.FirstOrDefaultAsync(f => f.Name == featureName);
                }

                //Get the feature
                Feature feature = FeatureManager.GetOrNull(featureName);
                if (feature == null)
                {
                    if (currentSetting != null)
                    {
                        await CompanyFeatureRepository.DeleteAsync(currentSetting);
                    }

                    return;
                }

                //Determine default value
                string defaultValue = feature.DefaultValue;

                //No need to store value if it's default
                if (value == defaultValue)
                {
                    if (currentSetting != null)
                    {
                        await CompanyFeatureRepository.DeleteAsync(currentSetting);
                    }

                    return;
                }

                //Insert/update the feature value
                if (currentSetting == null)
                {
                    await CompanyFeatureRepository.InsertAsync(
                        new CompanyFeatureSetting(company.Id, featureName, value));
                }
                else
                {
                    currentSetting.Value = value;
                }
            });
        }

        public virtual void SetFeatureValue(TCompany company, string featureName, string value)
        {
            UnitOfWorkManager.WithUnitOfWork(() =>
            {
                //No need to change if it's already equals to the current value
                if (GetFeatureValueOrNull(company.Id, featureName) == value)
                {
                    return;
                }

                //Get the current feature setting
                CompanyFeatureSetting currentSetting;
                using (UnitOfWorkManager.Current.EnableFilter(KontecgDataFilters.MayHaveCompany))
                using (UnitOfWorkManager.Current.SetCompanyId(company.Id))
                {
                    currentSetting = CompanyFeatureRepository.FirstOrDefault(f => f.Name == featureName);
                }

                //Get the feature
                Feature feature = FeatureManager.GetOrNull(featureName);
                if (feature == null)
                {
                    if (currentSetting != null)
                    {
                        CompanyFeatureRepository.Delete(currentSetting);
                    }

                    return;
                }

                //Determine default value
                string defaultValue = feature.DefaultValue;

                //No need to store value if it's default
                if (value == defaultValue)
                {
                    if (currentSetting != null)
                    {
                        CompanyFeatureRepository.Delete(currentSetting);
                    }

                    return;
                }

                //Insert/update the feature value
                if (currentSetting == null)
                {
                    CompanyFeatureRepository.Insert(new CompanyFeatureSetting(company.Id, featureName, value));
                }
                else
                {
                    currentSetting.Value = value;
                }
            });
        }

        /// <summary>
        ///     Resets all custom feature settings for a company.
        ///     Company will have features according to it's edition.
        /// </summary>
        /// <param name="companyId">Company Id</param>
        public virtual async Task ResetAllFeaturesAsync(int companyId)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (UnitOfWorkManager.Current.EnableFilter(KontecgDataFilters.MayHaveCompany))
                using (UnitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    await CompanyFeatureRepository.DeleteAsync(f => f.CompanyId == companyId);
                }
            });
        }

        /// <summary>
        ///     Resets all custom feature settings for a company.
        ///     Company will have features according to it's edition.
        /// </summary>
        /// <param name="companyId">Company Id</param>
        public virtual void ResetAllFeatures(int companyId)
        {
            UnitOfWorkManager.WithUnitOfWork(() =>
            {
                using (UnitOfWorkManager.Current.EnableFilter(KontecgDataFilters.MayHaveCompany))
                using (UnitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    CompanyFeatureRepository.Delete(f => f.CompanyId == companyId);
                }
            });
        }

        protected virtual async Task ValidateCompanyAsync(TCompany company)
        {
            await ValidateCompanyNameAsync(company.CompanyName);
        }

        protected virtual void ValidateCompany(TCompany company)
        {
            ValidateCompanyName(company.CompanyName);
        }

        protected virtual Task ValidateCompanyNameAsync(string companyName)
        {
            if (!Regex.IsMatch(companyName, KontecgCompanyBase.CompanyNameRegex))
            {
                throw new UserFriendlyException(L("InvalidCompanyName"));
            }

            return Task.FromResult(0);
        }

        protected virtual void ValidateCompanyName(string companyName)
        {
            if (!Regex.IsMatch(companyName, KontecgCompanyBase.CompanyNameRegex))
            {
                throw new UserFriendlyException(L("InvalidCompanyName"));
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
