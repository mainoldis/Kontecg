﻿using System;
using Microsoft.Extensions.Logging;
using ICastleLogger = Castle.Core.Logging.ILogger;

namespace Kontecg.Castle.Logging.MsLogging
{
    public class CastleMsLoggerAdapter : ILogger
    {
        private readonly ICastleLogger _castleLogger;

        public CastleMsLoggerAdapter(ICastleLogger castleLogger)
        {
            _castleLogger = castleLogger;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            string message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            _castleLogger.Log(logLevel, message, exception);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return _castleLogger.IsFatalEnabled;
                case LogLevel.Error:
                    return _castleLogger.IsErrorEnabled;
                case LogLevel.Warning:
                    return _castleLogger.IsWarnEnabled;
                case LogLevel.Information:
                    return _castleLogger.IsInfoEnabled;
                case LogLevel.Debug:
                case LogLevel.Trace: //Trace is not included in Castle Windsor
                    return _castleLogger.IsDebugEnabled;
                case LogLevel.None:
                    return false;
                default:
                    throw new ArgumentException($"{nameof(logLevel)} value is not implemented: " + logLevel);
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NullDisposable.Instance;
        }

        private class NullDisposable : IDisposable
        {
            public static readonly NullDisposable Instance = new();

            public void Dispose()
            {
            }
        }
    }
}
