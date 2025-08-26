using System;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Kontecg.Application.Features;
using Kontecg.Auditing;
using Kontecg.Authorization.Roles;
using Kontecg.Authorization.Users;
using Kontecg.Baseline.Configuration;
using Kontecg.Collections.Extensions;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.Localization;
using Kontecg.Localization.Dictionaries;
using Kontecg.Localization.Dictionaries.Xml;
using Kontecg.Modules;
using Kontecg.MultiCompany;
using Kontecg.Reflection;
using Kontecg.Reflection.Extensions;
using Kontecg.Threading.BackgroundWorkers;

namespace Kontecg.Baseline
{
    /// <summary>
    ///     Kontecg baseline core module.
    /// </summary>
    [DependsOn(typeof(KontecgKernelModule))]
    public class KontecgBaselineModule : KontecgModule
    {
        public override void PreInitialize()
        {
            IocManager
                .RegisterIfNot<IKontecgBaselineEntityTypes,
                    KontecgBaselineEntityTypes>(); //Registered on services.AddKontecgIdentity() for Kontecg.Core.

            IocManager.Register<IRoleManagementConfig, RoleManagementConfig>();
            IocManager.Register<IUserManagementConfig, UserManagementConfig>();
            IocManager.Register<ILanguageManagementConfig, LanguageManagementConfig>();
            IocManager.Register<IKontecgBaselineConfig, KontecgBaselineConfig>();

            Configuration.ReplaceService<ICompanyStore, CompanyStore>(DependencyLifeStyle.Transient);

            Configuration.Settings.Providers.Add<KontecgBaselineSettingProvider>();

            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    KontecgBaselineConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(KontecgBaselineModule).GetAssembly(), "Kontecg.Baseline.Localization.Source"
                    )));

            IocManager.IocContainer.Kernel.ComponentRegistered += Kernel_ComponentRegistered;

            AddIgnoredTypes();
        }

        public override void Initialize()
        {
            FillMissingEntityTypes();

            IocManager.Register<IMultiCompanyLocalizationDictionary, MultiCompanyLocalizationDictionary>(
                DependencyLifeStyle.Transient);
            IocManager.RegisterAssemblyByConvention(typeof(KontecgBaselineModule).GetAssembly());

            RegisterCompanyCache();
            RegisterUserTokenExpirationWorker();
        }

        public override void PostInitialize()
        {
            if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
            {
                using IDisposableDependencyObjectWrapper<IKontecgBaselineEntityTypes> entityTypes =
                    IocManager.ResolveAsDisposable<IKontecgBaselineEntityTypes>();
                Type implType = typeof(UserTokenExpirationWorker<,>)
                    .MakeGenericType(entityTypes.Object.Company, entityTypes.Object.User);
                IBackgroundWorkerManager workerManager = IocManager.Resolve<IBackgroundWorkerManager>();
                workerManager.Add(IocManager.Resolve(implType) as IBackgroundWorker);
            }
        }

        private void Kernel_ComponentRegistered(string key, IHandler handler)
        {
            if (typeof(IKontecgFeatureValueStore).IsAssignableFrom(handler.ComponentModel.Implementation) &&
                !IocManager.IsRegistered<IKontecgFeatureValueStore>())
            {
                IocManager.IocContainer.Register(
                    Component.For<IKontecgFeatureValueStore>()
                        .ImplementedBy(handler.ComponentModel.Implementation).Named("KontecgBaselineFeatureValueStore")
                        .LifestyleTransient()
                );
            }
        }

        private void AddIgnoredTypes()
        {
            Type[] ignoredTypes = new[]
            {
                typeof(AuditLog)
            };

            foreach (Type ignoredType in ignoredTypes)
            {
                Configuration.EntityHistory.IgnoredTypes.AddIfNotContains(ignoredType);
            }
        }

        private void FillMissingEntityTypes()
        {
            using IDisposableDependencyObjectWrapper<IKontecgBaselineEntityTypes> entityTypes =
                IocManager.ResolveAsDisposable<IKontecgBaselineEntityTypes>();
            if (entityTypes.Object.User != null &&
                entityTypes.Object.Role != null &&
                entityTypes.Object.Company != null)
            {
                return;
            }

            using IDisposableDependencyObjectWrapper<ITypeFinder> typeFinder =
                IocManager.ResolveAsDisposable<ITypeFinder>();
            Type[] types = typeFinder.Object.FindAll();
            entityTypes.Object.Company = types.FirstOrDefault(t =>
                typeof(KontecgCompanyBase).IsAssignableFrom(t) && !t.GetTypeInfo().IsAbstract);
            entityTypes.Object.Role = types.FirstOrDefault(t =>
                typeof(KontecgRoleBase).IsAssignableFrom(t) && !t.GetTypeInfo().IsAbstract);
            entityTypes.Object.User = types.FirstOrDefault(t =>
                typeof(KontecgUserBase).IsAssignableFrom(t) && !t.GetTypeInfo().IsAbstract);
        }

        private void RegisterCompanyCache()
        {
            if (IocManager.IsRegistered<ICompanyCache>())
                return;

            using IDisposableDependencyObjectWrapper<IKontecgBaselineEntityTypes> entityTypes =
                IocManager.ResolveAsDisposable<IKontecgBaselineEntityTypes>();

            Type implType = typeof(CompanyCache<,>)
                .MakeGenericType(entityTypes.Object.Company, entityTypes.Object.User);

            IocManager.Register(typeof(ICompanyCache), implType, DependencyLifeStyle.Transient);
        }

        private void RegisterUserTokenExpirationWorker()
        {
            using IDisposableDependencyObjectWrapper<IKontecgBaselineEntityTypes> entityTypes =
                IocManager.ResolveAsDisposable<IKontecgBaselineEntityTypes>();
            Type implType = typeof(UserTokenExpirationWorker<,>)
                .MakeGenericType(entityTypes.Object.Company, entityTypes.Object.User);
            IocManager.Register(implType);
        }
    }
}
