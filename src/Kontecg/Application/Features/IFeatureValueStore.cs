using System.Threading.Tasks;

namespace Kontecg.Application.Features
{
    /// <summary>
    ///     Defines a store to get a feature's value.
    /// </summary>
    public interface IFeatureValueStore
    {
        /// <summary>
        ///     Gets the feature value or null.
        /// </summary>
        /// <param name="companyId">The company id.</param>
        /// <param name="feature">The feature.</param>
        Task<string> GetValueOrNullAsync(int companyId, Feature feature);

        /// <summary>
        ///     Gets the feature value or null.
        /// </summary>
        /// <param name="companyId">The company id.</param>
        /// <param name="feature">The feature.</param>
        string GetValueOrNull(int companyId, Feature feature);
    }
}
