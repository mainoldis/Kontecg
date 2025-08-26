using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Kontecg.Application.Features;
using Kontecg.Authorization.Users;
using Kontecg.Collections.Extensions;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Services;
using Kontecg.Domain.Uow;
using Kontecg.Events.Bus.Entities;
using Kontecg.Events.Bus.Handlers;
using Kontecg.MultiCompany;

namespace Kontecg.Application.Clients
{
    public class KontecgClientManager<TCompany, TUser> : IDomainService,
        IEventHandler<EntityDeletedEventData<TCompany>>
        where TCompany : KontecgCompany<TUser>
        where TUser : KontecgUserBase
    {
        private readonly IKontecgFeatureValueStore _featureValueStore;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IClientFactory _clientFactory;

        public IQueryable<ClientInfo> Clients => ClientRepository.GetAll();

        public IFeatureManager FeatureManager { get; set; }

        protected IRepository<ClientInfo, string> ClientRepository { get; set; }

        public KontecgClientManager(
            IKontecgFeatureValueStore featureValueStore, 
            IUnitOfWorkManager unitOfWorkManager, 
            IRepository<ClientInfo, string> clientRepository,
            IClientFactory clientFactory)
        {
            _featureValueStore = featureValueStore;
            _unitOfWorkManager = unitOfWorkManager;
            _clientFactory = clientFactory;

            ClientRepository = clientRepository;
        }

        public Task<IQueryable<ClientInfo>> GetClientsAsync => ClientRepository.GetAllAsync();

        public virtual Task<string> GetFeatureValueOrNullAsync(string clientId, string featureName)
        {
            return _featureValueStore.GetClientValueOrNullAsync(clientId, featureName);
        }

        public virtual string GetFeatureValueOrNull(string clientId, string featureName)
        {
            return _featureValueStore.GetClientValueOrNull(clientId, featureName);
        }

        public virtual Task SetFeatureValueAsync(string clientId, string featureName, string value)
        {
            return _featureValueStore.SetClientFeatureValueAsync(clientId, featureName, value);
        }

        public virtual void SetFeatureValue(string clientId, string featureName, string value)
        {
            _featureValueStore.SetClientFeatureValue(clientId, featureName, value);
        }

        public virtual async Task<IReadOnlyList<NameValue>> GetFeatureValuesAsync(string clientId)
        {
            var values = new List<NameValue>();

            foreach (var feature in FeatureManager.GetAll())
            {
                values.Add(new NameValue(feature.Name, await GetFeatureValueOrNullAsync(clientId, feature.Name) ?? feature.DefaultValue));
            }

            return values;
        }

        public virtual IReadOnlyList<NameValue> GetFeatureValues(string clientId)
        {
            var values = new List<NameValue>();

            foreach (var feature in FeatureManager.GetAll())
            {
                values.Add(new NameValue(feature.Name, GetFeatureValueOrNull(clientId, feature.Name) ?? feature.DefaultValue));
            }

            return values;
        }

        public virtual async Task SetFeatureValuesAsync(string clientId, params NameValue[] values)
        {
            if (values.IsNullOrEmpty())
            {
                return;
            }

            foreach (var value in values)
            {
                await SetFeatureValueAsync(clientId, value.Name, value.Value);
            }
        }

        public virtual void SetFeatureValues(string clientId, params NameValue[] values)
        {
            if (values.IsNullOrEmpty())
            {
                return;
            }

            foreach (var value in values)
            {
                SetFeatureValue(clientId, value.Name, value.Value);
            }
        }

        public virtual async Task RegisterAsync()
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var asm = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

                ClientInfo client = _clientFactory.Create();

                var id = await FindByIdAsync(client.Id);
                if (id == null)
                {
                    await ClientRepository.InsertAsync(client);
                }
                    
                else
                {
                    id.IpAddress = client.IpAddress;
                    id.Name = client.Name;
                    id.Info = client.Info;
                    id.ExtensionData = client.ExtensionData;
                    id.Version = client.Version;

                    await ClientRepository.UpdateAsync(id);
                }
            });
        }

        public virtual void Register()
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                var asm = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

                ClientInfo client = _clientFactory.Create();
                var id = FindById(client.Id);
                if (id == null)
                {
                    ClientRepository.Insert(client);
                }
                else
                {
                    id.IpAddress = client.IpAddress;
                    id.Name = client.Name;
                    id.Info = client.Info;
                    id.ExtensionData = client.ExtensionData;
                    id.Version = client.Version;

                    ClientRepository.Update(id);
                }
            });
        }

        public virtual async Task<ClientInfo> FindByNameAsync(string name)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await ClientRepository.FirstOrDefaultAsync(client => client.Name == name);
            });
        }

        public virtual ClientInfo FindByName(string name)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                return ClientRepository.FirstOrDefault(client => client.Name == name);
            });
        }

        public virtual async Task<ClientInfo> FindByIdAsync(string id)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await ClientRepository.FirstOrDefaultAsync(id)
            );
        }

        public virtual ClientInfo FindById(string id)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
                ClientRepository.FirstOrDefault(id)
            );
        }

        public virtual async Task<ClientInfo> GetByIdAsync(string id)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await ClientRepository.GetAsync(id)
            );
        }

        public virtual ClientInfo GetById(string id)
        {
            return _unitOfWorkManager.WithUnitOfWork(() => ClientRepository.Get(id));
        }

        public virtual async Task DeleteAsync(ClientInfo client)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () => await ClientRepository.DeleteAsync(client));
        }

        public virtual void Delete(ClientInfo client)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                ClientRepository.Delete(client);
            });
        }

        /// <inheritdoc />
        public void HandleEvent(EntityDeletedEventData<TCompany> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.EnableFilter(KontecgDataFilters.MustHaveCompany))
                using (_unitOfWorkManager.Current.SetCompanyId(eventData.Entity.Id))
                {
                    var relatedClients = ClientRepository.GetAllList();
                    foreach (var relatedClient in relatedClients)
                    {
                        ClientRepository.Delete(relatedClient);
                    }
                }
            });
        }
    }
}
