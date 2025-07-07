using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Services;
using WelwiseChatModule.Runtime.Shared.Scripts.Network;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data
{
    public class ClientsDataProviderService : IClientsNicknamesDataProvider, IClientsNicknamesProviderService
    {
        public IReadOnlyDictionary<NetworkConnection, ClientData> Data => _data;

        public IReadOnlyDictionary<NetworkConnection, IClientNicknameData> ClientsNicknameDataProviders =>
            Data.ToDictionary(pair => pair.Key, pair => pair.Value.AccountData as IClientNicknameData);
        
        public IReadOnlyDictionary<NetworkConnection, string> Nicknames =>
            ClientsNicknameDataProviders.ToDictionary(pair => pair.Key, pair => pair.Value.Nickname);

        public event Action<NetworkConnection, IClientNicknameData> AddedNicknameData
        {
            add => _addedNicknameData += value;
            remove => _addedNicknameData -= value;
        }
        
        private Action<NetworkConnection, IClientNicknameData> _addedNicknameData;
        public event Action<NetworkConnection, ClientData> AddedData;
        
        private readonly Dictionary<NetworkConnection, ClientData> _data =
            new Dictionary<NetworkConnection, ClientData>();

        public void AddClientData(NetworkConnection networkConnection, ClientData clientData)
        {
            _data.AddOrAppoint(networkConnection, clientData);
            AddedData?.Invoke(networkConnection, clientData);
            _addedNicknameData?.Invoke(networkConnection, clientData.AccountData);
        }

        public void ClearClientsData() => _data.Clear();

        public void TryRemovingClientData(NetworkConnection networkConnection) => _data.Remove(networkConnection);
    }
}