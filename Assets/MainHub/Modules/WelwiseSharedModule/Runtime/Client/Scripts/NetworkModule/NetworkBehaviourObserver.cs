using System;
using FishNet.Connection;
using FishNet.Object;

namespace WelwiseSharedModule.Runtime.Client.Scripts.NetworkModule
{
    public class NetworkBehaviourObserver : NetworkBehaviour
    {
        public event Action<NetworkConnection> OnSpawnedServer;
        
        public override void OnSpawnServer(NetworkConnection connection)
        {
            OnSpawnedServer?.Invoke(connection);
            base.OnSpawnServer(connection);
        }
    }
}