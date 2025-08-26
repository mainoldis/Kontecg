using System;
using System.IO;
using System.Xml;
using Castle.Core.Logging;
using Kontecg.Reflection.Extensions;
using log4net;
using log4net.Config;
using log4net.Repository;
using log4net.Repository.Hierarchy;

namespace Kontecg.Castle.Logging.Log4Net
{
    public class Log4NetLoggerFactory : AbstractLoggerFactory
    {
        internal const string DefaultConfigFileName = "log4net.config";
        private readonly ILoggerRepository _loggerRepository;

        public Log4NetLoggerFactory()
            : this(DefaultConfigFileName)
        {
        }

        public Log4NetLoggerFactory(string configFileName)
        {
            _loggerRepository = LogManager.CreateRepository(
                typeof(Log4NetLoggerFactory).GetAssembly(),
                typeof(Hierarchy)
            );

            XmlDocument log4NetConfig = new XmlDocument();
            log4NetConfig.Load(File.OpenRead(configFileName));
            XmlConfigurator.Configure(_loggerRepository, log4NetConfig["log4net"]);
        }

        public Log4NetLoggerFactory(string configFileName, bool reloadOnChange)
        {
            _loggerRepository = LogManager.CreateRepository(
                typeof(Log4NetLoggerFactory).GetAssembly(),
                typeof(Hierarchy)
            );

            if (reloadOnChange)
            {
                XmlConfigurator.ConfigureAndWatch(_loggerRepository, new FileInfo(configFileName));
            }
            else
            {
                XmlDocument log4NetConfig = new XmlDocument();
                log4NetConfig.Load(File.OpenRead(configFileName));
                XmlConfigurator.Configure(_loggerRepository, log4NetConfig["log4net"]);
            }
        }

        public override ILogger Create(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return new Log4NetLogger(LogManager.GetLogger(_loggerRepository.Name, name), this);
        }

        public override ILogger Create(string name, LoggerLevel level)
        {
            throw new NotSupportedException(
                "Logger levels cannot be set at runtime. Please review your configuration file.");
        }
    }
}
