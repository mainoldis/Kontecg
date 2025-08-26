using System;
using System.Threading;

namespace Kontecg.EFCore
{
    public class KontecgEfCoreCurrentDbContext
    {
        private readonly AsyncLocal<KontecgDbContext> _current = new AsyncLocal<KontecgDbContext>();

        public KontecgDbContext Context => _current.Value;

        public IDisposable Use(KontecgDbContext context)
        {
            var previousValue = Context;
            _current.Value = context;
            return new DisposeAction(() =>
            {
                _current.Value = previousValue;
            });
        }
    }
}
