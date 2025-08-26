using System;
using System.Collections.Generic;
using Kontecg.Dependency;

namespace Kontecg.MemoryDb
{
    public class MemoryDatabase : ISingletonDependency
    {
        private readonly Dictionary<Type, object> _sets;

        private readonly object _syncObj = new();

        public MemoryDatabase()
        {
            _sets = new Dictionary<Type, object>();
        }

        public List<TEntity> Set<TEntity>()
        {
            Type entityType = typeof(TEntity);

            lock (_syncObj)
            {
                if (!_sets.ContainsKey(entityType))
                {
                    _sets[entityType] = new List<TEntity>();
                }

                return _sets[entityType] as List<TEntity>;
            }
        }
    }
}
