using FishNet.Broadcast;
using FishNet.Connection;
using UnityEngine;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts.Network
{
    public struct PlayerInitializationDependencies : IBroadcast
    {
        public readonly GameObject Player;
        public readonly NetworkConnection Connection;

        public PlayerInitializationDependencies(GameObject player, NetworkConnection connection)
        {
            Player = player;
            Connection = connection;
        }
    }
}