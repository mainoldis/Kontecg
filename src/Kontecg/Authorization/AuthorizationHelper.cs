using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Kontecg.Application.Features;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.Localization;
using Kontecg.Reflection;
using Kontecg.Runtime.Session;

namespace Kontecg.Authorization
{
    public class AuthorizationHelper : IAuthorizationHelper, ITransientDependency
    {
        private readonly IAuthorizationConfiguration _authConfiguration;

        private readonly IFeatureChecker _featureChecker;

        public AuthorizationHelper(IFeatureChecker featureChecker, IAuthorizationConfiguration authConfiguration)
        {
            _featureChecker = featureChecker;
            _authConfiguration = authConfiguration;
            KontecgSession = NullKontecgSession.Instance;
            PermissionChecker = NullPermissionChecker.Instance;
            LocalizationManager = NullLocalizationManager.Instance;
        }

        public IKontecgSession KontecgSession { get; set; }
        public IPermissionChecker PermissionChecker { get; set; }
        public ILocalizationManager LocalizationManager { get; set; }

        public virtual async Task AuthorizeAsync(IEnumerable<IKontecgAuthorizeAttribute> authorizeAttributes)
        {
            if (!_authConfiguration.IsEnabled)
            {
                return;
            }

            if (!KontecgSession.UserId.HasValue)
            {
                throw new KontecgAuthorizationException(
                    LocalizationManager.GetString(KontecgConsts.LocalizationSourceName,
                        "CurrentUserDidNotLoginToTheApplication")
                );
            }

            foreach (IKontecgAuthorizeAttribute authorizeAttribute in authorizeAttributes)
            {
                await PermissionChecker.AuthorizeAsync(authorizeAttribute.RequireAllPermissions,
                    authorizeAttribute.Permissions);
            }
        }

        public virtual void Authorize(IEnumerable<IKontecgAuthorizeAttribute> authorizeAttributes)
        {
            if (!_authConfiguration.IsEnabled)
            {
                return;
            }

            if (!KontecgSession.UserId.HasValue)
            {
                throw new KontecgAuthorizationException(
                    LocalizationManager.GetString(KontecgConsts.LocalizationSourceName,
                        "CurrentUserDidNotLoginToTheApplication")
                );
            }

            foreach (IKontecgAuthorizeAttribute authorizeAttribute in authorizeAttributes)
            {
                PermissionChecker.Authorize(authorizeAttribute.RequireAllPermissions, authorizeAttribute.Permissions);
            }
        }

        public virtual async Task AuthorizeAsync(MethodInfo methodInfo, Type type)
        {
            await CheckFeaturesAsync(methodInfo, type);
            await CheckPermissionsAsync(methodInfo, type);
        }

        public virtual void Authorize(MethodInfo methodInfo, Type type)
        {
            CheckFeatures(methodInfo, type);
            CheckPermissions(methodInfo, type);
        }

        protected virtual async Task CheckFeaturesAsync(MethodInfo methodInfo, Type type)
        {
            List<RequiresFeatureAttribute> featureAttributes =
                ReflectionHelper.GetAttributesOfMemberAndType<RequiresFeatureAttribute>(methodInfo, type);

            if (featureAttributes.Count == 0)
            {
                return;
            }

            foreach (RequiresFeatureAttribute featureAttribute in featureAttributes)
            {
                await _featureChecker.CheckEnabledAsync(featureAttribute.RequiresAll, featureAttribute.Features);
            }
        }

        protected virtual void CheckFeatures(MethodInfo methodInfo, Type type)
        {
            List<RequiresFeatureAttribute> featureAttributes =
                ReflectionHelper.GetAttributesOfMemberAndType<RequiresFeatureAttribute>(methodInfo, type);

            if (featureAttributes.Count == 0)
            {
                return;
            }

            foreach (RequiresFeatureAttribute featureAttribute in featureAttributes)
            {
                _featureChecker.CheckEnabled(featureAttribute.RequiresAll, featureAttribute.Features);
            }
        }

        protected virtual async Task CheckPermissionsAsync(MethodInfo methodInfo, Type type)
        {
            if (!_authConfiguration.IsEnabled)
            {
                return;
            }

            if (AllowAnonymous(methodInfo, type))
            {
                return;
            }

            if (ReflectionHelper.IsPropertyGetterSetterMethod(methodInfo, type))
            {
                return;
            }

            if (!methodInfo.IsPublic &&
                !methodInfo.GetCustomAttributes().OfType<IKontecgAuthorizeAttribute>().Any())
            {
                return;
            }

            IKontecgAuthorizeAttribute[] authorizeAttributes =
                ReflectionHelper
                    .GetAttributesOfMemberAndType(methodInfo, type)
                    .OfType<IKontecgAuthorizeAttribute>()
                    .ToArray();

            if (!authorizeAttributes.Any())
            {
                return;
            }

            await AuthorizeAsync(authorizeAttributes);
        }

        protected virtual void CheckPermissions(MethodInfo methodInfo, Type type)
        {
            if (!_authConfiguration.IsEnabled)
            {
                return;
            }

            if (AllowAnonymous(methodInfo, type))
            {
                return;
            }

            if (ReflectionHelper.IsPropertyGetterSetterMethod(methodInfo, type))
            {
                return;
            }

            if (!methodInfo.IsPublic &&
                !methodInfo.GetCustomAttributes().OfType<IKontecgAuthorizeAttribute>().Any())
            {
                return;
            }

            IKontecgAuthorizeAttribute[] authorizeAttributes =
                ReflectionHelper
                    .GetAttributesOfMemberAndType(methodInfo, type)
                    .OfType<IKontecgAuthorizeAttribute>()
                    .ToArray();

            if (!authorizeAttributes.Any())
            {
                return;
            }

            Authorize(authorizeAttributes);
        }

        private static bool AllowAnonymous(MemberInfo memberInfo, Type type)
        {
            return ReflectionHelper
                .GetAttributesOfMemberAndType(memberInfo, type)
                .OfType<IKontecgAllowAnonymousAttribute>()
                .Any();
        }
    }
}
