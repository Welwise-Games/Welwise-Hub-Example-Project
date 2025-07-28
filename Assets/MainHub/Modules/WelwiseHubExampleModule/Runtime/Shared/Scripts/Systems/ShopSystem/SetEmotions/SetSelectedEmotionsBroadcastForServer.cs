using System.Collections.Generic;
using FishNet.Broadcast;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts.Systems.ShopSystem.SetEmotions
{
    public struct SetSelectedEmotionsBroadcastForServer : IBroadcast
    {
        public List<SelectedEmotionData> Data { get; set; }

        public SetSelectedEmotionsBroadcastForServer(List<SelectedEmotionData> data)
        {
            Data = data;
        }
    }
}