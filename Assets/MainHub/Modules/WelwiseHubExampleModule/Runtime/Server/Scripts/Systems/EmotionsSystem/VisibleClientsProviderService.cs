using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.HubSystem;
using WelwiseSharedModule.Runtime.Server.Scripts;

namespace WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.EmotionsSystem
{
    public class VisibleClientsProviderService : IVisibleClientsProviderService
    {
        public IReadOnlyCollection<NetworkConnection> GetVisibleClientsForClient(NetworkConnection networkConnection) =>
            _hubsProviderService.HubByPlayerNetworkConnection.GetValueOrDefault(networkConnection)?.ConnectedClientsNetworkConnections
                .Where(connection => connection != networkConnection).ToHashSet();

        private readonly HubsProviderService _hubsProviderService;

        public VisibleClientsProviderService(HubsProviderService hubsProviderService)
        {
            _hubsProviderService = hubsProviderService;
        }
    }
}