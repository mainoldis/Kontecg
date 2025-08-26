using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Linq.Expressions;

namespace Kontecg.EFCore
{
    public class KontecgCompiledQueryCacheKeyGenerator : ICompiledQueryCacheKeyGenerator
    {
        protected ICompiledQueryCacheKeyGenerator InnerCompiledQueryCacheKeyGenerator { get; }
        protected ICurrentDbContext CurrentContext { get; }

        public KontecgCompiledQueryCacheKeyGenerator(
            ICompiledQueryCacheKeyGenerator innerCompiledQueryCacheKeyGenerator,
            ICurrentDbContext currentContext)
        {
            InnerCompiledQueryCacheKeyGenerator = innerCompiledQueryCacheKeyGenerator;
            CurrentContext = currentContext;
        }

        public virtual object GenerateCacheKey(Expression query, bool async)
        {
            var cacheKey = InnerCompiledQueryCacheKeyGenerator.GenerateCacheKey(query, async);
            if (CurrentContext.Context is KontecgDbContext kontecgDbContext)
            {
                return new KontecgCompiledQueryCacheKey(cacheKey, kontecgDbContext.GetCompiledQueryCacheKey());
            }

            return cacheKey;
        }

        private readonly struct KontecgCompiledQueryCacheKey : IEquatable<KontecgCompiledQueryCacheKey>
        {
            private readonly object _compiledQueryCacheKey;
            private readonly string _currentFilterCacheKey;

            public KontecgCompiledQueryCacheKey(object compiledQueryCacheKey, string currentFilterCacheKey)
            {
                _compiledQueryCacheKey = compiledQueryCacheKey;
                _currentFilterCacheKey = currentFilterCacheKey;
            }

            public override bool Equals(object obj)
            {
                return obj is KontecgCompiledQueryCacheKey key && Equals(key);
            }

            public bool Equals(KontecgCompiledQueryCacheKey other)
            {
                return _compiledQueryCacheKey.Equals(other._compiledQueryCacheKey) &&
                       _currentFilterCacheKey == other._currentFilterCacheKey;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(_compiledQueryCacheKey, _currentFilterCacheKey);
            }
        }
    }
}
