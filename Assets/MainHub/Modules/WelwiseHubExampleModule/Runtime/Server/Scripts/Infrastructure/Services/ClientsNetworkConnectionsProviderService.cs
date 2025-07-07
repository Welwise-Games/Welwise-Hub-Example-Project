using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using Modules.WelwiseChatModule.Runtime.Server.Scripts;
using WelwiseChatModule.Runtime.Shared.Scripts.Network;
using WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.HubSystem;

namespace WelwiseHubExampleModule.Runtime.Server.Scripts.Infrastructure.Services
{
    public class ClientsNetworkConnectionsProviderService : IClientsNetworkConnectionsProviderService
    {
        private readonly HubsProviderService _hubsProviderService;

        public ClientsNetworkConnectionsProviderService(HubsProviderService hubsProviderService)
        {
            _hubsProviderService = hubsProviderService;
        }

        public IReadOnlyCollection<NetworkConnection> GetClientsNetworkConnections(NetworkConnection networkConnection,
            ChatZone chatZone)
        {
            if (chatZone != ChatZone.Hub && chatZone != ChatZone.Test) return null;
            
            _hubsProviderService.HubByPlayerNetworkConnection.ToDictionary(hub => hub.Key,
                hub => hub.Value.ConnectedClientsNetworkConnections).TryGetValue(networkConnection, out var connections);

            return connections;

        }
    }
}