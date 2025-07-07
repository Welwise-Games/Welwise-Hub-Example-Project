using FishNet.Broadcast;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts.Network
{
    public struct SettingClientCustomizationDataBroadcastForServer : IBroadcast
    {
        public readonly CustomizationData CustomizationData;

        public SettingClientCustomizationDataBroadcastForServer(CustomizationData customizationData)
        {
            CustomizationData = customizationData;
 
        }
    }
}