using System;
using System.Collections.Concurrent;
using Castle.Core.Logging;
using JetBrains.Annotations;
using Kontecg.Collections.Extensions;

namespace Kontecg.Runtime.Remoting
{
    public class DataContextAmbientScopeProvider<T> : IAmbientScopeProvider<T>
    {
        private static readonly ConcurrentDictionary<string, ScopeItem> ScopeDictionary = new();

        private readonly IAmbientDataContext _dataContext;

        public DataContextAmbientScopeProvider([NotNull] IAmbientDataContext dataContext)
        {
            Check.NotNull(dataContext, nameof(dataContext));

            _dataContext = dataContext;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public T GetValue(string contextKey)
        {
            ScopeItem item = GetCurrentItem(contextKey);
            if (item == null)
            {
                return default;
            }

            return item.Value;
        }

        public IDisposable BeginScope(string contextKey, T value)
        {
            ScopeItem item = new ScopeItem(value, GetCurrentItem(contextKey));

            if (!ScopeDictionary.TryAdd(item.Id, item))
            {
                throw new KontecgException("Can not add item! ScopeDictionary.TryAdd returns false!");
            }

            _dataContext.SetData(contextKey, item.Id);

            return new DisposeAction(() =>
            {
                ScopeDictionary.TryRemove(item.Id, out item);

                if (item.Outer == null)
                {
                    _dataContext.SetData(contextKey, null);
                    return;
                }

                _dataContext.SetData(contextKey, item.Outer.Id);
            });
        }

        private ScopeItem GetCurrentItem(string contextKey)
        {
            return _dataContext.GetData(contextKey) is string objKey ? ScopeDictionary.GetOrDefault(objKey) : null;
        }

        private class ScopeItem
        {
            public ScopeItem(T value, ScopeItem outer = null)
            {
                Id = Guid.NewGuid().ToString();

                Value = value;
                Outer = outer;
            }

            public string Id { get; }

            public ScopeItem Outer { get; }

            public T Value { get; }
        }
    }
}
