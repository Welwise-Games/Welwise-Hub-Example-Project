using FishNet.Broadcast;
using FishNet.Connection;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts.Network
{
    public struct SettingClientCustomizationDataBroadcastForClient : IBroadcast
    {
        public readonly CustomizationData CustomizationData;
        public readonly NetworkConnection DataOwnerNetworkConnection;

        public SettingClientCustomizationDataBroadcastForClient(CustomizationData customizationData, NetworkConnection dataOwnerNetworkConnection)
        {
            CustomizationData = customizationData;
            DataOwnerNetworkConnection = dataOwnerNetworkConnection;
        }
    }
}