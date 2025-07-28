using FishNet.Broadcast;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts.Network
{
    public struct SetClientCustomizationDataBroadcastForServer : IBroadcast
    {
        public readonly CustomizationData CustomizationData;

        public SetClientCustomizationDataBroadcastForServer(CustomizationData customizationData)
        {
            CustomizationData = customizationData;
        }
    }
}