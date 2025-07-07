using System.Collections.Generic;
using FishNet.Broadcast;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts.Network
{
    public struct PlayersInitializationBroadcast : IBroadcast
    {
        public readonly List<PlayerInitializationDependencies> Dependencies;

        public PlayersInitializationBroadcast(List<PlayerInitializationDependencies> dependencies)
        {
            Dependencies = dependencies;
        }
    }
}