using System.Collections.Generic;
using FishNet.Broadcast;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts.Systems.ShopSystem.SettingEmotions
{
    public struct SetSelectedEmotionsBroadcast : IBroadcast
    {
        public readonly List<SelectedEmotionData> Data;

        public SetSelectedEmotionsBroadcast(List<SelectedEmotionData> data)
        {
            Data = data;
        }
    }
}