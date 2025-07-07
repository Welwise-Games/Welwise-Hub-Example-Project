using FishNet.Broadcast;
using FishNet.Connection;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data;
using WelwiseSharedModule.Runtime.Shared.Scripts;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts.Network
{
    public struct ClientServicesInitializationBroadcast : IBroadcast
    {
        public readonly string SerializedClientData;
        public readonly NetworkConnection DataOwnerNetworkConnection;

        public ClientServicesInitializationBroadcast(ClientData clientData, NetworkConnection dataOwnerNetworkConnection)
        {
            DataOwnerNetworkConnection = dataOwnerNetworkConnection;
            SerializedClientData = clientData.GetJsonSerializedObjectWithoutNulls();
        }
    }
}