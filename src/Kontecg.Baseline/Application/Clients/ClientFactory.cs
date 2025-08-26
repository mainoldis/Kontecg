using System;
using System.IO;
using System.Linq;
using Castle.Core.Logging;
using Kontecg.Auditing;
using Kontecg.Collections.Extensions;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.Domain.Entities;
using Kontecg.Extensions;
using Kontecg.Modules;
using Kontecg.MultiCompany;
using Kontecg.Runtime.Session;

namespace Kontecg.Application.Clients
{
    public class ClientFactory : IClientFactory, ITransientDependency
    {
        private readonly IKontecgModuleManager _moduleManager;
        private readonly IClientInfoProvider _clientInfoProvider;
        private readonly IMultiCompanyConfig _multiCompanyConfig;

        public ClientFactory(
            IClientInfoProvider clientInfoProvider,
            IKontecgModuleManager moduleManager,
            IMultiCompanyConfig multiCompanyConfig)
        {
            _clientInfoProvider = clientInfoProvider;
            _moduleManager = moduleManager;
            _multiCompanyConfig = multiCompanyConfig;

            KontecgSession = NullKontecgSession.Instance;
            Logger = NullLogger.Instance;
        }

        public IKontecgSession KontecgSession { get; set; }

        public ILogger Logger { get; set; }
        
        public ClientInfo Create()
        {
            var clientInfo = new ClientInfo
            {
                CompanyId = GetCompanyId()
            };

            try
            {
                Fill(clientInfo);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString(), ex);
            }

            return clientInfo;
        }

        private void Fill(ClientInfo clientInfo)
        {
            clientInfo.Id = _clientInfoProvider.ClientId;
            clientInfo.IpAddress = _clientInfoProvider.ClientIpAddress;
            clientInfo.Name = _clientInfoProvider.ComputerName;
            clientInfo.Info = _clientInfoProvider.ClientInfo;
            clientInfo.Version = _clientInfoProvider.Version;
            var plugins = _moduleManager.Modules
                .SortByDependencies(x => x.Dependencies)
                .Where(m => m.IsLoadedAsPlugIn)
                .Select(m => new
                {
                    m.Name, m.Description,
                    Path = Path.GetDirectoryName(m.Assembly.Location).EnsureEndsWith(Path.DirectorySeparatorChar),
                    m.Version
                })
                .ToArray();

            var modules = _moduleManager.Modules
                .SortByDependencies(x => x.Dependencies)
                .Select(m => new
                {
                    m.Name,
                    m.Description,
                    m.Version
                })
                .ToArray();

            clientInfo.SetData("Modules", modules);//Loaded modules
            clientInfo.SetData("Required", Array.Empty<string>());//Only register plugins
            clientInfo.SetData("Installed", plugins);//Only register plugins
        }

        private int GetCompanyId()
        {
            return _multiCompanyConfig.IsEnabled && KontecgSession.CompanyId.HasValue
                ? KontecgSession.CompanyId.Value
                : MultiCompanyConsts.DefaultCompanyId;
        }
    }
}
