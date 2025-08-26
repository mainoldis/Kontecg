using System.Threading.Tasks;
using Kontecg.Extensions;

namespace Kontecg.Configuration
{
    /// <summary>
    ///     Extension methods for <see cref="ISettingManager" />.
    /// </summary>
    public static class SettingManagerExtensions
    {
        /// <summary>
        ///     Gets value of a setting in given type (<typeparamref name="T" />).
        /// </summary>
        /// <typeparam name="T">Type of the setting to get</typeparam>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <returns>Value of the setting</returns>
        public static async Task<T> GetSettingValueAsync<T>(this ISettingManager settingManager, string name)
            where T : struct
        {
            return (await settingManager.GetSettingValueAsync(name)).To<T>();
        }

        /// <summary>
        ///     Gets value of a setting in given type (<typeparamref name="T" />).
        /// </summary>
        /// <typeparam name="T">Type of the setting to get</typeparam>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <returns>Value of the setting</returns>
        public static T GetSettingValue<T>(this ISettingManager settingManager, string name)
            where T : struct
        {
            return settingManager.GetSettingValue(name).To<T>();
        }

        /// <summary>
        ///     Gets current value of a setting for the application level.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <returns>Current value of the setting for the application</returns>
        public static async Task<T> GetSettingValueForApplicationAsync<T>(this ISettingManager settingManager,
            string name)
            where T : struct
        {
            return (await settingManager.GetSettingValueForApplicationAsync(name)).To<T>();
        }

        /// <summary>
        ///     Gets current value of a setting for the application level.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <returns>Current value of the setting for the application</returns>
        public static T GetSettingValueForApplication<T>(this ISettingManager settingManager, string name)
            where T : struct
        {
            return settingManager.GetSettingValueForApplication(name).To<T>();
        }

        /// <summary>
        ///     Gets current value of a setting for a company level.
        ///     It gets the setting value, overwritten by given company.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="companyId">Company id</param>
        /// <returns>Current value of the setting</returns>
        public static async Task<T> GetSettingValueForCompanyAsync<T>(this ISettingManager settingManager, string name,
            int companyId)
            where T : struct
        {
            return (await settingManager.GetSettingValueForCompanyAsync(name, companyId)).To<T>();
        }

        /// <summary>
        ///     Gets current value of a setting for a company level.
        ///     It gets the setting value, overwritten by given company.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="companyId">Company id</param>
        /// <returns>Current value of the setting</returns>
        public static T GetSettingValueForCompany<T>(this ISettingManager settingManager, string name, int companyId)
            where T : struct
        {
            return settingManager.GetSettingValueForCompany(name, companyId).To<T>();
        }

        /// <summary>
        ///     Gets current value of a setting for a user level.
        ///     It gets the setting value, overwritten by given company and user.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="companyId">Company id</param>
        /// <param name="userId">User id</param>
        /// <returns>Current value of the setting for the user</returns>
        public static async Task<T> GetSettingValueForUserAsync<T>(this ISettingManager settingManager, string name,
            int? companyId, long userId)
            where T : struct
        {
            return (await settingManager.GetSettingValueForUserAsync(name, companyId, userId)).To<T>();
        }

        /// <summary>
        ///     Gets current value of a setting for a user level.
        ///     It gets the setting value, overwritten by given company and user.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="companyId">Company id</param>
        /// <param name="userId">User id</param>
        /// <returns>Current value of the setting for the user</returns>
        public static T GetSettingValueForUser<T>(this ISettingManager settingManager, string name, int? companyId,
            long userId)
            where T : struct
        {
            return settingManager.GetSettingValueForUser(name, companyId, userId).To<T>();
        }

        /// <summary>
        ///     Gets current value of a setting for a user level.
        ///     It gets the setting value, overwritten by given company and user.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="user">User</param>
        /// <returns>Current value of the setting for the user</returns>
        public static async Task<T> GetSettingValueForUserAsync<T>(this ISettingManager settingManager, string name,
            UserIdentifier user)
            where T : struct
        {
            return (await settingManager.GetSettingValueForUserAsync(name, user)).To<T>();
        }

        /// <summary>
        ///     Gets current value of a setting for a user level.
        ///     It gets the setting value, overwritten by given company and user.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="user">User</param>
        /// <returns>Current value of the setting for the user</returns>
        public static T GetSettingValueForUser<T>(this ISettingManager settingManager, string name, UserIdentifier user)
            where T : struct
        {
            return settingManager.GetSettingValueForUser(name, user).To<T>();
        }
    }
}
