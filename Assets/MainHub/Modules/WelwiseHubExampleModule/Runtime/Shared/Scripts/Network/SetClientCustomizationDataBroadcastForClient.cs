using FishNet.Broadcast;
using FishNet.Connection;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts.Network
{
    public struct SetClientCustomizationDataBroadcastForClient : IBroadcast
    {
        public readonly CustomizationData CustomizationData;
        public readonly NetworkConnection DataOwnerNetworkConnection;

        public SetClientCustomizationDataBroadcastForClient(CustomizationData customizationData, NetworkConnection dataOwnerNetworkConnection)
        {
            CustomizationData = customizationData;
            DataOwnerNetworkConnection = dataOwnerNetworkConnection;
        }
    }
}