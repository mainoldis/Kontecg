using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;

namespace Kontecg.Configuration
{
    /// <summary>
    ///     Implements <see cref="ISettingStore" />.
    /// </summary>
    public class SettingStore : ISettingStore, ITransientDependency
    {
        private readonly IRepository<Setting, long> _settingRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public SettingStore(
            IRepository<Setting, long> settingRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _settingRepository = settingRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public virtual async Task<List<SettingInfo>> GetAllListAsync(int? companyId, long? userId)
        {
            /* Combined SetCompanyId and DisableFilter for backward compatibility.
             * SetCompanyId switches database (for company) if needed.
             * DisableFilter and Where condition ensures to work even if companyId is null for single db approach.
             */

            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    using (_unitOfWorkManager.Current.DisableFilter(KontecgDataFilters.MayHaveCompany))
                    {
                        List<Setting> settingList = await _settingRepository.GetAllListAsync(s =>
                            s.UserId == userId && s.CompanyId == companyId
                        );

                        return settingList
                            .Select(s => s.ToSettingInfo())
                            .ToList();
                    }
                }
            });
        }

        public virtual List<SettingInfo> GetAllList(int? companyId, long? userId)
        {
            /* Combined SetCompanyId and DisableFilter for backward compatibility.
             * SetCompanyId switches database (for company) if needed.
             * DisableFilter and Where condition ensures to work even if companyId is null for single db approach.
             */

            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    using (_unitOfWorkManager.Current.DisableFilter(KontecgDataFilters.MayHaveCompany))
                    {
                        return _settingRepository.GetAllList(s =>
                                s.UserId == userId && s.CompanyId == companyId
                            )
                            .Select(s => s.ToSettingInfo())
                            .ToList();
                    }
                }
            });
        }

        public virtual async Task<SettingInfo> GetSettingOrNullAsync(int? companyId, long? userId, string name)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    using (_unitOfWorkManager.Current.DisableFilter(KontecgDataFilters.MayHaveCompany))
                    {
                        Setting settingInfo = await _settingRepository.FirstOrDefaultAsync(s =>
                            s.UserId == userId && s.Name == name && s.CompanyId == companyId
                        );

                        return settingInfo.ToSettingInfo();
                    }
                }
            });
        }

        public virtual SettingInfo GetSettingOrNull(int? companyId, long? userId, string name)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    using (_unitOfWorkManager.Current.DisableFilter(KontecgDataFilters.MayHaveCompany))
                    {
                        return _settingRepository.FirstOrDefault(s =>
                            s.UserId == userId && s.Name == name && s.CompanyId == companyId
                        ).ToSettingInfo();
                    }
                }
            });
        }

        public virtual async Task DeleteAsync(SettingInfo settingInfo)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(settingInfo.CompanyId))
                {
                    using (_unitOfWorkManager.Current.DisableFilter(KontecgDataFilters.MayHaveCompany))
                    {
                        await _settingRepository.DeleteAsync(
                            s => s.UserId == settingInfo.UserId && s.Name == settingInfo.Name &&
                                 s.CompanyId == settingInfo.CompanyId
                        );

                        await _unitOfWorkManager.Current.SaveChangesAsync();
                    }
                }
            });
        }

        public virtual void Delete(SettingInfo settingInfo)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(settingInfo.CompanyId))
                {
                    using (_unitOfWorkManager.Current.DisableFilter(KontecgDataFilters.MayHaveCompany))
                    {
                        _settingRepository.Delete(
                            s => s.UserId == settingInfo.UserId && s.Name == settingInfo.Name &&
                                 s.CompanyId == settingInfo.CompanyId
                        );

                        _unitOfWorkManager.Current.SaveChanges();
                    }
                }
            });
        }

        public virtual async Task CreateAsync(SettingInfo settingInfo)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(settingInfo.CompanyId))
                {
                    using (_unitOfWorkManager.Current.DisableFilter(KontecgDataFilters.MayHaveCompany))
                    {
                        await _settingRepository.InsertAsync(settingInfo.ToSetting());
                        await _unitOfWorkManager.Current.SaveChangesAsync();
                    }
                }
            });
        }

        public virtual void Create(SettingInfo settingInfo)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(settingInfo.CompanyId))
                {
                    using (_unitOfWorkManager.Current.DisableFilter(KontecgDataFilters.MayHaveCompany))
                    {
                        _settingRepository.Insert(settingInfo.ToSetting());
                        _unitOfWorkManager.Current.SaveChanges();
                    }
                }
            });
        }

        public virtual async Task UpdateAsync(SettingInfo settingInfo)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(settingInfo.CompanyId))
                {
                    using (_unitOfWorkManager.Current.DisableFilter(KontecgDataFilters.MayHaveCompany))
                    {
                        Setting setting = await _settingRepository.FirstOrDefaultAsync(
                            s => s.CompanyId == settingInfo.CompanyId &&
                                 s.UserId == settingInfo.UserId &&
                                 s.Name == settingInfo.Name
                        );

                        if (setting != null)
                        {
                            setting.Value = settingInfo.Value;
                        }

                        await _unitOfWorkManager.Current.SaveChangesAsync();
                    }
                }
            });
        }

        public virtual void Update(SettingInfo settingInfo)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(settingInfo.CompanyId))
                {
                    using (_unitOfWorkManager.Current.DisableFilter(KontecgDataFilters.MayHaveCompany))
                    {
                        Setting setting = _settingRepository.FirstOrDefault(
                            s => s.CompanyId == settingInfo.CompanyId &&
                                 s.UserId == settingInfo.UserId &&
                                 s.Name == settingInfo.Name
                        );

                        if (setting != null)
                        {
                            setting.Value = settingInfo.Value;
                        }

                        _unitOfWorkManager.Current.SaveChanges();
                    }
                }
            });
        }
    }
}
