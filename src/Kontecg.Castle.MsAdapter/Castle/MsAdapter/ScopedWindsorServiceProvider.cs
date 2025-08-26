﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Castle.Windsor;
using Microsoft.Extensions.DependencyInjection;

namespace Kontecg.Castle.MsAdapter
{
    /// <summary>
    ///     Implements <see cref="IServiceProvider" />.
    /// </summary>
    public class ScopedWindsorServiceProvider : IServiceProvider, ISupportRequiredService
    {
        private static readonly AsyncLocal<bool> _isInResolving = new();
        private readonly IWindsorContainer _container;

        public ScopedWindsorServiceProvider(IWindsorContainer container,
            MsLifetimeScopeProvider msLifetimeScopeProvider)
        {
            _container = container;
            OwnMsLifetimeScope = msLifetimeScopeProvider.LifetimeScope;
        }

        protected IMsLifetimeScope OwnMsLifetimeScope { get; }

        public static bool IsInResolving
        {
            get => _isInResolving.Value;
            set => _isInResolving.Value = value;
        }

        public object GetService(Type serviceType)
        {
            return GetServiceInternal(serviceType, true);
        }

        public object GetRequiredService(Type serviceType)
        {
            return GetServiceInternal(serviceType, false);
        }

        private object GetServiceInternal(Type serviceType, bool isOptional)
        {
            using (MsLifetimeScope.Using(OwnMsLifetimeScope))
            {
                bool isAlreadyInResolving = IsInResolving;

                if (!isAlreadyInResolving)
                {
                    IsInResolving = true;
                }

                object instance = null;
                try
                {
                    return instance = ResolveInstanceOrNull(serviceType, isOptional);
                }
                finally
                {
                    if (!isAlreadyInResolving)
                    {
                        if (instance != null)
                        {
                            OwnMsLifetimeScope?.AddInstance(instance);
                        }

                        IsInResolving = false;
                    }
                }
            }
        }

        private object ResolveInstanceOrNull(Type serviceType, bool isOptional)
        {
            //Check if given service is directly registered
            if (_container.Kernel.HasComponent(serviceType))
            {
                return _container.Resolve(serviceType);
            }

            // Check if requested IEnumerable<TService>
            // MS uses GetService<IEnumerable<TService>>() to get a collection.
            // This must be resolved with IWindsorContainer.ResolveAll();

            if (serviceType.GetTypeInfo().IsGenericType &&
                serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                Array allObjects = _container.ResolveAll(serviceType.GenericTypeArguments[0]);
                Array.Reverse(allObjects);
                return allObjects;
            }

            if (isOptional)
            {
                //Not found
                return null;
            }

            //Let Castle Windsor throws exception since the service is not registered!
            return _container.Resolve(serviceType);
        }
    }
}
