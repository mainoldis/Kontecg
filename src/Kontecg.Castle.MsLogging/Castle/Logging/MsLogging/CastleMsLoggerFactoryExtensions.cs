using Microsoft.Extensions.Logging;
using ICastleLoggerFactory = Castle.Core.Logging.ILoggerFactory;

namespace Kontecg.Castle.Logging.MsLogging
{
    public static class CastleMsLoggerFactoryExtensions
    {
        public static ILoggerFactory AddCastleLogger(this ILoggerFactory factory,
            ICastleLoggerFactory castleLoggerFactory)
        {
            factory.AddProvider(new CastleMsLoggerProvider(castleLoggerFactory));
            return factory;
        }
    }
}
