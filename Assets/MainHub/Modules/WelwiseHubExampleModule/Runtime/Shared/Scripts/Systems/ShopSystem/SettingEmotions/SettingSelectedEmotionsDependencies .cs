using System.Collections.Generic;
using FishNet.Broadcast;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts.Systems.ShopSystem.SettingEmotions
{
    public struct SettingSelectedEmotionsDependencies : IBroadcast
    {
        public readonly List<SelectedEmotionData> Data;

        public SettingSelectedEmotionsDependencies(List<SelectedEmotionData> data)
        {
            Data = data;
        }
    }
}