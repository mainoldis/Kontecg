using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Kontecg.Logging;
using Kontecg.Threading;

namespace Kontecg.Configuration
{
    /// <summary>
    ///     Implements default behavior for ISettingStore.
    ///     Only <see cref="GetSettingOrNullAsync" /> method is implemented and it gets setting's value
    ///     from application's configuration file if exists, or returns null if not.
    /// </summary>
    public class DefaultConfigSettingStore : ISettingStore
    {
        private DefaultConfigSettingStore()
        {
        }

        /// <summary>
        ///     Gets singleton instance.
        /// </summary>
        public static DefaultConfigSettingStore Instance { get; } = new();

        public Task<SettingInfo> GetSettingOrNullAsync(int? companyId, long? userId, string name)
        {
            string value = ConfigurationManager.AppSettings[name];

            if (value == null)
            {
                return Task.FromResult<SettingInfo>(null);
            }

            return Task.FromResult(new SettingInfo(companyId, userId, name, value));
        }

        public SettingInfo GetSettingOrNull(int? companyId, long? userId, string name)
        {
            string value = ConfigurationManager.AppSettings[name];

            if (value == null)
            {
                return null;
            }

            return new SettingInfo(companyId, userId, name, value);
        }

        /// <inheritdoc />
        public Task DeleteAsync(SettingInfo setting)
        {
            LogHelper.Logger.Warn(
                "ISettingStore is not implemented, using DefaultConfigSettingStore which does not support DeleteAsync.");
            return KontecgTaskCache.CompletedTask;
        }

        /// <inheritdoc />
        public void Delete(SettingInfo setting)
        {
            LogHelper.Logger.Warn(
                "ISettingStore is not implemented, using DefaultConfigSettingStore which does not support DeleteAsync.");
        }

        /// <inheritdoc />
        public Task CreateAsync(SettingInfo setting)
        {
            LogHelper.Logger.Warn(
                "ISettingStore is not implemented, using DefaultConfigSettingStore which does not support CreateAsync.");
            return KontecgTaskCache.CompletedTask;
        }

        /// <inheritdoc />
        public void Create(SettingInfo setting)
        {
            LogHelper.Logger.Warn(
                "ISettingStore is not implemented, using DefaultConfigSettingStore which does not support CreateAsync.");
        }

        /// <inheritdoc />
        public Task UpdateAsync(SettingInfo setting)
        {
            LogHelper.Logger.Warn(
                "ISettingStore is not implemented, using DefaultConfigSettingStore which does not support UpdateAsync.");
            return KontecgTaskCache.CompletedTask;
        }

        /// <inheritdoc />
        public void Update(SettingInfo setting)
        {
            LogHelper.Logger.Warn(
                "ISettingStore is not implemented, using DefaultConfigSettingStore which does not support UpdateAsync.");
        }

        /// <inheritdoc />
        public Task<List<SettingInfo>> GetAllListAsync(int? companyId, long? userId)
        {
            LogHelper.Logger.Warn(
                "ISettingStore is not implemented, using DefaultConfigSettingStore which does not support GetAllListAsync.");
            return Task.FromResult(new List<SettingInfo>());
        }

        /// <inheritdoc />
        public List<SettingInfo> GetAllList(int? companyId, long? userId)
        {
            LogHelper.Logger.Warn(
                "ISettingStore is not implemented, using DefaultConfigSettingStore which does not support GetAllListAsync.");
            return new List<SettingInfo>();
        }
    }
}
