using System.Collections.Generic;
using FishNet.Broadcast;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts.Network
{
    public struct ClientsServicesInitializationBroadcast : IBroadcast
    {
        public readonly List<ClientServicesInitializationBroadcast> ClientsServicesInitializationBroadcasts;

        public ClientsServicesInitializationBroadcast(List<ClientServicesInitializationBroadcast> broadcasts)
        {
            ClientsServicesInitializationBroadcasts = broadcasts;
        }
    }
}