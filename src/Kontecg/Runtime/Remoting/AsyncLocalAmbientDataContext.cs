using System.Collections.Concurrent;
using System.Threading;
using Kontecg.Dependency;

namespace Kontecg.Runtime.Remoting
{
    public class AsyncLocalAmbientDataContext : IAmbientDataContext, ISingletonDependency
    {
        private static readonly ConcurrentDictionary<string, AsyncLocal<object>> AsyncLocalDictionary = new();

        public void SetData(string key, object value)
        {
            AsyncLocal<object> asyncLocal = AsyncLocalDictionary.GetOrAdd(key, k => new AsyncLocal<object>());
            asyncLocal.Value = value;
        }

        public object GetData(string key)
        {
            AsyncLocal<object> asyncLocal = AsyncLocalDictionary.GetOrAdd(key, k => new AsyncLocal<object>());
            return asyncLocal.Value;
        }
    }
}
