using System;
using System.Reflection;
using System.Threading.Tasks;
using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Kontecg.Modules;
using Kontecg.Runtime.Session;
using Kontecg.TestBase.Runtime.Session;

namespace Kontecg.TestBase
{
    /// <summary>
    ///     This is the base class for all tests integrated to Kontecg.
    /// </summary>
    public abstract class KontecgIntegratedTestBase<TStartupModule> : IDisposable
        where TStartupModule : KontecgModule
    {
        protected KontecgIntegratedTestBase(bool initializeKontecg = true, IIocManager localIocManager = null)
        {
            LocalIocManager = localIocManager ?? new IocManager();

            KontecgBootstrapper = KontecgBootstrapper.Create<TStartupModule>(options =>
            {
                options.IocManager = LocalIocManager;
            });

            if (initializeKontecg)
            {
                InitializeKontecg();
            }
        }

        /// <summary>
        ///     Local <see cref="IIocManager" /> used for this test.
        /// </summary>
        protected IIocManager LocalIocManager { get; }

        protected KontecgBootstrapper KontecgBootstrapper { get; }

        /// <summary>
        ///     Gets Session object. Can be used to change current user and company in tests.
        /// </summary>
        protected TestKontecgSession KontecgSession { get; private set; }

        public virtual void Dispose()
        {
            KontecgBootstrapper.Dispose();
            LocalIocManager.Dispose();
        }

        protected void InitializeKontecg()
        {
            LocalIocManager.RegisterIfNot<IKontecgSession, TestKontecgSession>();

            PreInitialize();

            KontecgBootstrapper.Initialize();

            PostInitialize();

            KontecgSession = LocalIocManager.Resolve<TestKontecgSession>();
        }

        /// <summary>
        ///     This method can be overrided to replace some services with fakes.
        /// </summary>
        protected virtual void PreInitialize()
        {
        }

        /// <summary>
        ///     This method can be overrided to replace some services with fakes.
        /// </summary>
        protected virtual void PostInitialize()
        {
        }

        /// <summary>
        ///     A shortcut to resolve an object from <see cref="LocalIocManager" />.
        ///     Also registers <typeparamref name="T" /> as transient if it's not registered before.
        /// </summary>
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <returns>The object instance</returns>
        protected T Resolve<T>()
        {
            EnsureClassRegistered(typeof(T));
            return LocalIocManager.Resolve<T>();
        }

        /// <summary>
        ///     A shortcut to resolve an object from <see cref="LocalIocManager" />.
        ///     Also registers <typeparamref name="T" /> as transient if it's not registered before.
        /// </summary>
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The object instance</returns>
        protected T Resolve<T>(object argumentsAsAnonymousType)
        {
            EnsureClassRegistered(typeof(T));
            return LocalIocManager.Resolve<T>(argumentsAsAnonymousType);
        }

        /// <summary>
        ///     A shortcut to resolve an object from <see cref="LocalIocManager" />.
        ///     Also registers <paramref name="type" /> as transient if it's not registered before.
        /// </summary>
        /// <param name="type">Type of the object to get</param>
        /// <returns>The object instance</returns>
        protected object Resolve(Type type)
        {
            EnsureClassRegistered(type);
            return LocalIocManager.Resolve(type);
        }

        /// <summary>
        ///     A shortcut to resolve an object from <see cref="LocalIocManager" />.
        ///     Also registers <paramref name="type" /> as transient if it's not registered before.
        /// </summary>
        /// <param name="type">Type of the object to get</param>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The object instance</returns>
        protected object Resolve(Type type, object argumentsAsAnonymousType)
        {
            EnsureClassRegistered(type);
            return LocalIocManager.Resolve(type, argumentsAsAnonymousType);
        }

        /// <summary>
        ///     Registers given type if it's not registered before.
        /// </summary>
        /// <param name="type">Type to check and register</param>
        /// <param name="lifeStyle">Lifestyle</param>
        protected void EnsureClassRegistered(Type type, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Transient)
        {
            if (!LocalIocManager.IsRegistered(type))
            {
                if (!type.GetTypeInfo().IsClass || type.GetTypeInfo().IsAbstract)
                {
                    throw new KontecgException("Can not register " + type.Name +
                                               ". It should be a non-abstract class. If not, it should be registered before.");
                }

                LocalIocManager.Register(type, lifeStyle);
            }
        }

        protected virtual void WithUnitOfWork(Action action, UnitOfWorkOptions options = null)
        {
            using IDisposableDependencyObjectWrapper<IUnitOfWorkManager> uowManager =
                LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>();
            using IUnitOfWorkCompleteHandle uow = uowManager.Object.Begin(options ?? new UnitOfWorkOptions());
            action();
            uow.Complete();
        }

        protected virtual void WithUnitOfWork(int? companyId, Action action, UnitOfWorkOptions options = null)
        {
            using IDisposableDependencyObjectWrapper<IUnitOfWorkManager> uowManager =
                LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>();
            using IUnitOfWorkCompleteHandle uow = uowManager.Object.Begin(options ?? new UnitOfWorkOptions());
            using (uowManager.Object.Current.SetCompanyId(companyId))
            {
                action();
                uow.Complete();
            }
        }

        protected virtual async Task WithUnitOfWorkAsync(Func<Task> action, UnitOfWorkOptions options = null)
        {
            using IDisposableDependencyObjectWrapper<IUnitOfWorkManager> uowManager =
                LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>();
            using IUnitOfWorkCompleteHandle uow = uowManager.Object.Begin(options ?? new UnitOfWorkOptions());
            await action();
            await uow.CompleteAsync();
        }

        protected async Task WithUnitOfWorkAsync(int? companyId, Func<Task> action, UnitOfWorkOptions options = null)
        {
            using IDisposableDependencyObjectWrapper<IUnitOfWorkManager> uowManager =
                LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>();
            using IUnitOfWorkCompleteHandle uow = uowManager.Object.Begin(options ?? new UnitOfWorkOptions());
            using (uowManager.Object.Current.SetCompanyId(companyId))
            {
                await action();
                await uow.CompleteAsync();
            }
        }
    }
}
