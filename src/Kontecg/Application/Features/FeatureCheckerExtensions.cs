using System.Linq;
using System.Threading.Tasks;
using Kontecg.Authorization;
using Kontecg.Collections.Extensions;
using Kontecg.Dependency;
using Kontecg.Localization;

namespace Kontecg.Application.Features
{
    /// <summary>
    ///     Some extension methods for <see cref="IFeatureChecker" />.
    /// </summary>
    public static class FeatureCheckerExtensions
    {
        /// <summary>
        ///     Used to check if one or all of the given features are enabled.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker" /> instance</param>
        /// <param name="requiresAll">True, to require that all the given features are enabled. False, to require one or more.</param>
        /// <param name="featureNames">Names of the features</param>
        public static async Task<bool> IsEnabledAsync(this IFeatureChecker featureChecker, bool requiresAll,
            params string[] featureNames)
        {
            if (featureNames.IsNullOrEmpty())
            {
                return true;
            }

            if (requiresAll)
            {
                foreach (string featureName in featureNames)
                {
                    if (!await featureChecker.IsEnabledAsync(featureName))
                    {
                        return false;
                    }
                }

                return true;
            }

            foreach (string featureName in featureNames)
            {
                if (await featureChecker.IsEnabledAsync(featureName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Used to check if one or all of the given features are enabled.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker" /> instance</param>
        /// <param name="requiresAll">True, to require that all the given features are enabled. False, to require one or more.</param>
        /// <param name="featureNames">Names of the features</param>
        public static bool IsEnabled(this IFeatureChecker featureChecker, bool requiresAll,
            params string[] featureNames)
        {
            if (featureNames.IsNullOrEmpty())
            {
                return true;
            }

            return requiresAll
                ? featureNames.All(featureChecker.IsEnabled)
                : featureNames.Any(featureChecker.IsEnabled);
        }


        /// <summary>
        ///     Used to check if one or all of the given features are enabled.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker" /> instance</param>
        /// <param name="companyId">Company id</param>
        /// <param name="requiresAll">True, to require that all the given features are enabled. False, to require one or more.</param>
        /// <param name="featureNames">Names of the features</param>
        public static async Task<bool> IsEnabledAsync(this IFeatureChecker featureChecker, int companyId,
            bool requiresAll, params string[] featureNames)
        {
            if (featureNames.IsNullOrEmpty())
            {
                return true;
            }

            if (requiresAll)
            {
                foreach (string featureName in featureNames)
                {
                    if (!await featureChecker.IsEnabledAsync(companyId, featureName))
                    {
                        return false;
                    }
                }

                return true;
            }

            foreach (string featureName in featureNames)
            {
                if (await featureChecker.IsEnabledAsync(companyId, featureName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Used to check if one or all of the given features are enabled.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker" /> instance</param>
        /// <param name="companyId">Company id</param>
        /// <param name="requiresAll">True, to require that all the given features are enabled. False, to require one or more.</param>
        /// <param name="featureNames">Names of the features</param>
        public static bool IsEnabled(this IFeatureChecker featureChecker, int companyId, bool requiresAll,
            params string[] featureNames)
        {
            if (featureNames.IsNullOrEmpty())
            {
                return true;
            }

            return requiresAll
                ? featureNames.All(featureName => featureChecker.IsEnabled(companyId, featureName))
                : featureNames.Any(featureName => featureChecker.IsEnabled(companyId, featureName));
        }

        /// <summary>
        ///     Checks if a given feature is enabled. Throws <see cref="KontecgAuthorizationException" /> if not.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker" /> instance</param>
        /// <param name="featureName">Unique feature name</param>
        public static async Task CheckEnabledAsync(this IFeatureChecker featureChecker, string featureName)
        {
            string[] localizedFeatureNames = LocalizeFeatureNames(featureChecker, new[] {featureName});

            if (!await featureChecker.IsEnabledAsync(featureName))
            {
                throw new KontecgAuthorizationException(string.Format(
                    L(
                        featureChecker,
                        "FeatureIsNotEnabled",
                        "Feature is not enabled: {0}"
                    ),
                    localizedFeatureNames.First()
                ));
            }
        }

        /// <summary>
        ///     Checks if a given feature is enabled. Throws <see cref="KontecgAuthorizationException" /> if not.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker" /> instance</param>
        /// <param name="featureName">Unique feature name</param>
        public static void CheckEnabled(this IFeatureChecker featureChecker, string featureName)
        {
            string[] localizedFeatureNames = LocalizeFeatureNames(featureChecker, new[] {featureName});

            if (!featureChecker.IsEnabled(featureName))
            {
                throw new KontecgAuthorizationException(string.Format(
                    L(
                        featureChecker,
                        "FeatureIsNotEnabled",
                        "Feature is not enabled: {0}"
                    ),
                    localizedFeatureNames.First()
                ));
            }
        }

        /// <summary>
        ///     Checks if one or all of the given features are enabled. Throws <see cref="KontecgAuthorizationException" /> if not.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker" /> instance</param>
        /// <param name="requiresAll">True, to require that all the given features are enabled. False, to require one or more.</param>
        /// <param name="featureNames">Names of the features</param>
        public static async Task CheckEnabledAsync(this IFeatureChecker featureChecker, bool requiresAll,
            params string[] featureNames)
        {
            if (featureNames.IsNullOrEmpty())
            {
                return;
            }

            string[] localizedFeatureNames = LocalizeFeatureNames(featureChecker, featureNames);

            if (requiresAll)
            {
                foreach (string featureName in featureNames)
                {
                    if (!await featureChecker.IsEnabledAsync(featureName))
                    {
                        throw new KontecgAuthorizationException(
                            string.Format(
                                L(
                                    featureChecker,
                                    "AllOfTheseFeaturesMustBeEnabled",
                                    "Required features are not enabled. All of these features must be enabled: {0}"
                                ),
                                string.Join(", ", localizedFeatureNames)
                            )
                        );
                    }
                }
            }
            else
            {
                foreach (string featureName in featureNames)
                {
                    if (await featureChecker.IsEnabledAsync(featureName))
                    {
                        return;
                    }
                }

                throw new KontecgAuthorizationException(
                    string.Format(
                        L(
                            featureChecker,
                            "AtLeastOneOfTheseFeaturesMustBeEnabled",
                            "Required features are not enabled. At least one of these features must be enabled: {0}"
                        ),
                        string.Join(", ", localizedFeatureNames)
                    )
                );
            }
        }

        /// <summary>
        ///     Checks if one or all of the given features are enabled. Throws <see cref="KontecgAuthorizationException" /> if not.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker" /> instance</param>
        /// <param name="requiresAll">True, to require that all the given features are enabled. False, to require one or more.</param>
        /// <param name="featureNames">Names of the features</param>
        public static void CheckEnabled(this IFeatureChecker featureChecker, bool requiresAll,
            params string[] featureNames)
        {
            if (featureNames.IsNullOrEmpty())
            {
                return;
            }

            string[] localizedFeatureNames = LocalizeFeatureNames(featureChecker, featureNames);

            if (requiresAll)
            {
                if (featureNames.Any(featureName => !featureChecker.IsEnabled(featureName)))
                {
                    throw new KontecgAuthorizationException(
                        string.Format(
                            L(
                                featureChecker,
                                "AllOfTheseFeaturesMustBeEnabled",
                                "Required features are not enabled. All of these features must be enabled: {0}"
                            ),
                            string.Join(", ", localizedFeatureNames)
                        )
                    );
                }
            }
            else
            {
                if (featureNames.Any(featureChecker.IsEnabled))
                {
                    return;
                }

                throw new KontecgAuthorizationException(
                    string.Format(
                        L(
                            featureChecker,
                            "AtLeastOneOfTheseFeaturesMustBeEnabled",
                            "Required features are not enabled. At least one of these features must be enabled: {0}"
                        ),
                        string.Join(", ", localizedFeatureNames)
                    )
                );
            }
        }

        /// <summary>
        ///     Checks if one or all of the given features are enabled. Throws <see cref="KontecgAuthorizationException" /> if not.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker" /> instance</param>
        /// <param name="companyId">Company id</param>
        /// <param name="requiresAll">True, to require that all the given features are enabled. False, to require one or more.</param>
        /// <param name="featureNames">Names of the features</param>
        public static async Task CheckEnabledAsync(this IFeatureChecker featureChecker, int companyId, bool requiresAll,
            params string[] featureNames)
        {
            if (featureNames.IsNullOrEmpty())
            {
                return;
            }

            if (requiresAll)
            {
                foreach (string featureName in featureNames)
                {
                    if (!await featureChecker.IsEnabledAsync(companyId, featureName))
                    {
                        throw new KontecgAuthorizationException(
                            string.Format(
                                L(
                                    featureChecker,
                                    "AllOfTheseFeaturesMustBeEnabled",
                                    "Required features are not enabled. All of these features must be enabled: {0}"
                                ),
                                string.Join(", ", featureNames)
                            )
                        );
                    }
                }
            }
            else
            {
                foreach (string featureName in featureNames)
                {
                    if (await featureChecker.IsEnabledAsync(companyId, featureName))
                    {
                        return;
                    }
                }

                throw new KontecgAuthorizationException(
                    string.Format(
                        L(
                            featureChecker,
                            "AtLeastOneOfTheseFeaturesMustBeEnabled",
                            "Required features are not enabled. At least one of these features must be enabled: {0}"
                        ),
                        string.Join(", ", featureNames)
                    )
                );
            }
        }

        /// <summary>
        ///     Checks if one or all of the given features are enabled. Throws <see cref="KontecgAuthorizationException" /> if not.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker" /> instance</param>
        /// <param name="companyId">Company id</param>
        /// <param name="requiresAll">True, to require that all the given features are enabled. False, to require one or more.</param>
        /// <param name="featureNames">Names of the features</param>
        public static void CheckEnabled(this IFeatureChecker featureChecker, int companyId, bool requiresAll,
            params string[] featureNames)
        {
            if (featureNames.IsNullOrEmpty())
            {
                return;
            }

            if (requiresAll)
            {
                foreach (string featureName in featureNames)
                {
                    if (!featureChecker.IsEnabled(companyId, featureName))
                    {
                        throw new KontecgAuthorizationException(
                            string.Format(
                                L(
                                    featureChecker,
                                    "AllOfTheseFeaturesMustBeEnabled",
                                    "Required features are not enabled. All of these features must be enabled: {0}"
                                ),
                                string.Join(", ", featureNames)
                            )
                        );
                    }
                }
            }
            else
            {
                foreach (string featureName in featureNames)
                {
                    if (featureChecker.IsEnabled(companyId, featureName))
                    {
                        return;
                    }
                }

                throw new KontecgAuthorizationException(
                    string.Format(
                        L(
                            featureChecker,
                            "AtLeastOneOfTheseFeaturesMustBeEnabled",
                            "Required features are not enabled. At least one of these features must be enabled: {0}"
                        ),
                        string.Join(", ", featureNames)
                    )
                );
            }
        }

        public static string L(IFeatureChecker featureChecker, string name, string defaultValue)
        {
            if (!(featureChecker is IIocManagerAccessor))
            {
                return defaultValue;
            }

            IIocManager iocManager = (featureChecker as IIocManagerAccessor).IocManager;
            using IDisposableDependencyObjectWrapper<ILocalizationManager> localizationManager =
                iocManager.ResolveAsDisposable<ILocalizationManager>();
            return localizationManager.Object.GetString(KontecgConsts.LocalizationSourceName, name);
        }

        public static string[] LocalizeFeatureNames(IFeatureChecker featureChecker, string[] featureNames)
        {
            if (!(featureChecker is IIocManagerAccessor))
            {
                return featureNames;
            }

            IIocManager iocManager = (featureChecker as IIocManagerAccessor).IocManager;
            using IDisposableDependencyObjectWrapper<ILocalizationContext> localizationContext =
                iocManager.ResolveAsDisposable<ILocalizationContext>();
            using IDisposableDependencyObjectWrapper<IFeatureManager> featureManager =
                iocManager.ResolveAsDisposable<IFeatureManager>();
            return featureNames.Select(featureName =>
            {
                Feature feature = featureManager.Object.GetOrNull(featureName);
                return feature?.DisplayName == null
                    ? featureName
                    : feature.DisplayName.Localize(localizationContext.Object);
            }).ToArray();
        }
    }
}
