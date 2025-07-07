using System.Collections.Generic;
using FishNet.Connection;
using WelwiseChatModule.Runtime.Shared.Scripts.Network;

namespace Modules.WelwiseChatModule.Runtime.Server.Scripts
{
    public interface IClientsNetworkConnectionsProviderService
    {
        IReadOnlyCollection<NetworkConnection> GetClientsNetworkConnections(NetworkConnection networkConnection, ChatZone chatZone);
    }
}