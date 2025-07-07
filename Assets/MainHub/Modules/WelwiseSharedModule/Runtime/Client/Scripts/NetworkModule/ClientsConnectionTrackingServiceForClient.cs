using System;
using FishNet.Connection;
using FishNet.Managing.Client;
using FishNet.Transporting;

namespace WelwiseSharedModule.Runtime.Client.Scripts.NetworkModule
{
    public class ClientsConnectionTrackingServiceForClient
    {
        public event Action<NetworkConnection> Disconnected, Connected;
        public event Action OwnerDisconnected, OwnerConnected;
        
        private readonly ClientManager _clientManager;

        public ClientsConnectionTrackingServiceForClient(ClientManager clientManager)
        {
            _clientManager = clientManager;
        }

        public void InvokeConnectedActionForOwner()
        {
            Connected?.Invoke(SharedNetworkTools.OwnerConnection);
            OwnerConnected?.Invoke();
        }

        public void TryInvokingDisconnectedActionForOwner(ClientConnectionStateArgs args)
        {
            if (args.ConnectionState != LocalConnectionState.Stopping) return;
            
            Disconnected?.Invoke(SharedNetworkTools.OwnerConnection);
            OwnerDisconnected?.Invoke();
        }
        
        public void InvokeActionByConnectionStateForNotOwner(RemoteConnectionStateArgs args)
        {
            var clientConnectionId = args.ConnectionId;

            if (args.ConnectionState == RemoteConnectionState.Started)
                Connected?.Invoke(_clientManager.Clients[clientConnectionId]);
            else
                Disconnected?.Invoke(_clientManager.Clients[clientConnectionId]);
        }
    }
}