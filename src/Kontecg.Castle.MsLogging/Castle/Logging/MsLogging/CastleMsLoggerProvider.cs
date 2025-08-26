using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using ICastleLoggerFactory = Castle.Core.Logging.ILoggerFactory;

namespace Kontecg.Castle.Logging.MsLogging
{
    public class CastleMsLoggerProvider : ILoggerProvider
    {
        private readonly ICastleLoggerFactory _castleLoggerFactory;
        private readonly ConcurrentDictionary<string, CastleMsLoggerAdapter> _loggers;

        public CastleMsLoggerProvider(ICastleLoggerFactory castleLoggerFactory)
        {
            _castleLoggerFactory = castleLoggerFactory;
            _loggers = new ConcurrentDictionary<string, CastleMsLoggerAdapter>();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(
                categoryName,
                name => new CastleMsLoggerAdapter(_castleLoggerFactory.Create(name))
            );
        }

        public void Dispose()
        {
        }
    }
}
