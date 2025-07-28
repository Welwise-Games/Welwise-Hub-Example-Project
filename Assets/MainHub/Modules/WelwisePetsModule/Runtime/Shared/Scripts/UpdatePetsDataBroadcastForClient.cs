using FishNet.Broadcast;
using FishNet.Connection;

namespace WelwisePetsModule.Runtime.Shared.Scripts
{
    public struct UpdatePetsDataBroadcastForClient : IBroadcast
    {
        public readonly SelectedPetsData Data;
        public readonly NetworkConnection NetworkConnection;

        public UpdatePetsDataBroadcastForClient(SelectedPetsData data, NetworkConnection networkConnection)
        {
            Data = data;
            NetworkConnection = networkConnection;
        }
    }
}