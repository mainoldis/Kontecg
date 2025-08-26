using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Kontecg.Application.Features;
using Kontecg.Authorization;
using Kontecg.Collections.Extensions;
using Kontecg.Dependency;

namespace Kontecg.Notifications
{
    /// <summary>
    ///     Implements <see cref="INotificationDefinitionManager" />.
    /// </summary>
    internal class NotificationDefinitionManager : INotificationDefinitionManager, ISingletonDependency
    {
        private readonly INotificationConfiguration _configuration;
        private readonly IocManager _iocManager;

        private readonly IDictionary<string, NotificationDefinition> _notificationDefinitions;

        public NotificationDefinitionManager(
            IocManager iocManager,
            INotificationConfiguration configuration)
        {
            _configuration = configuration;
            _iocManager = iocManager;

            _notificationDefinitions = new Dictionary<string, NotificationDefinition>();
        }

        public void Add(NotificationDefinition notificationDefinition)
        {
            if (_notificationDefinitions.ContainsKey(notificationDefinition.Name))
            {
                throw new KontecgInitializationException(
                    "There is already a notification definition with given name: " + notificationDefinition.Name +
                    ". Notification names must be unique!");
            }

            _notificationDefinitions[notificationDefinition.Name] = notificationDefinition;
        }

        public NotificationDefinition Get(string name)
        {
            NotificationDefinition definition = GetOrNull(name);
            if (definition == null)
            {
                throw new KontecgException("There is no notification definition with given name: " + name);
            }

            return definition;
        }

        public NotificationDefinition GetOrNull(string name)
        {
            return _notificationDefinitions.GetOrDefault(name);
        }

        public void Remove(string name)
        {
            _notificationDefinitions.Remove(name);
        }

        public IReadOnlyList<NotificationDefinition> GetAll()
        {
            return _notificationDefinitions.Values.ToImmutableList();
        }

        public async Task<bool> IsAvailableAsync(string name, UserIdentifier user)
        {
            NotificationDefinition notificationDefinition = GetOrNull(name);
            if (notificationDefinition == null)
            {
                return true;
            }

            if (notificationDefinition.FeatureDependency != null)
            {
                using IDisposableDependencyObjectWrapper<FeatureDependencyContext> featureDependencyContext =
                    _iocManager.ResolveAsDisposable<FeatureDependencyContext>();
                featureDependencyContext.Object.CompanyId = user.CompanyId;

                if (!await notificationDefinition.FeatureDependency.IsSatisfiedAsync(
                        featureDependencyContext.Object))
                {
                    return false;
                }
            }

            if (notificationDefinition.PermissionDependency != null)
            {
                using IDisposableDependencyObjectWrapper<PermissionDependencyContext> permissionDependencyContext =
                    _iocManager.ResolveAsDisposable<PermissionDependencyContext>();
                permissionDependencyContext.Object.User = user;

                if (!await notificationDefinition.PermissionDependency.IsSatisfiedAsync(permissionDependencyContext
                        .Object))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsAvailable(string name, UserIdentifier user)
        {
            NotificationDefinition notificationDefinition = GetOrNull(name);
            if (notificationDefinition == null)
            {
                return true;
            }

            if (notificationDefinition.FeatureDependency != null)
            {
                using IDisposableDependencyObjectWrapper<FeatureDependencyContext> featureDependencyContext =
                    _iocManager.ResolveAsDisposable<FeatureDependencyContext>();
                featureDependencyContext.Object.CompanyId = user.CompanyId;

                if (!notificationDefinition.FeatureDependency.IsSatisfied(featureDependencyContext.Object))
                {
                    return false;
                }
            }

            if (notificationDefinition.PermissionDependency != null)
            {
                using IDisposableDependencyObjectWrapper<PermissionDependencyContext> permissionDependencyContext =
                    _iocManager.ResolveAsDisposable<PermissionDependencyContext>();
                permissionDependencyContext.Object.User = user;

                if (!notificationDefinition.PermissionDependency.IsSatisfied(permissionDependencyContext.Object))
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<IReadOnlyList<NotificationDefinition>> GetAllAvailableAsync(UserIdentifier user)
        {
            List<NotificationDefinition> availableDefinitions = new List<NotificationDefinition>();

            using (IDisposableDependencyObjectWrapper<PermissionDependencyContext> permissionDependencyContext =
                   _iocManager.ResolveAsDisposable<PermissionDependencyContext>())
            {
                permissionDependencyContext.Object.User = user;

                using (IDisposableDependencyObjectWrapper<FeatureDependencyContext> featureDependencyContext =
                       _iocManager.ResolveAsDisposable<FeatureDependencyContext>())
                {
                    featureDependencyContext.Object.CompanyId = user.CompanyId;

                    foreach (NotificationDefinition notificationDefinition in GetAll())
                    {
                        if (notificationDefinition.PermissionDependency != null &&
                            !await notificationDefinition.PermissionDependency.IsSatisfiedAsync(
                                permissionDependencyContext.Object))
                        {
                            continue;
                        }

                        if (user.CompanyId.HasValue &&
                            notificationDefinition.FeatureDependency != null &&
                            !await notificationDefinition.FeatureDependency.IsSatisfiedAsync(featureDependencyContext
                                .Object))
                        {
                            continue;
                        }

                        availableDefinitions.Add(notificationDefinition);
                    }
                }
            }

            return availableDefinitions.ToImmutableList();
        }

        public IReadOnlyList<NotificationDefinition> GetAllAvailable(UserIdentifier user)
        {
            List<NotificationDefinition> availableDefinitions = new List<NotificationDefinition>();

            using (IDisposableDependencyObjectWrapper<PermissionDependencyContext> permissionDependencyContext =
                   _iocManager.ResolveAsDisposable<PermissionDependencyContext>())
            {
                permissionDependencyContext.Object.User = user;

                using (IDisposableDependencyObjectWrapper<FeatureDependencyContext> featureDependencyContext =
                       _iocManager.ResolveAsDisposable<FeatureDependencyContext>())
                {
                    featureDependencyContext.Object.CompanyId = user.CompanyId;

                    foreach (NotificationDefinition notificationDefinition in GetAll())
                    {
                        if (notificationDefinition.PermissionDependency != null &&
                            !notificationDefinition.PermissionDependency.IsSatisfied(permissionDependencyContext.Object)
                           )
                        {
                            continue;
                        }

                        if (user.CompanyId.HasValue &&
                            notificationDefinition.FeatureDependency != null &&
                            !notificationDefinition.FeatureDependency.IsSatisfied(featureDependencyContext.Object))
                        {
                            continue;
                        }

                        availableDefinitions.Add(notificationDefinition);
                    }
                }
            }

            return availableDefinitions.ToImmutableList();
        }

        public void Initialize()
        {
            NotificationDefinitionContext context = new NotificationDefinitionContext(this);

            foreach (Type providerType in _configuration.Providers)
            {
                using IDisposableDependencyObjectWrapper<NotificationProvider> provider =
                    _iocManager.ResolveAsDisposable<NotificationProvider>(providerType);
                provider.Object.SetNotifications(context);
            }
        }
    }
}
