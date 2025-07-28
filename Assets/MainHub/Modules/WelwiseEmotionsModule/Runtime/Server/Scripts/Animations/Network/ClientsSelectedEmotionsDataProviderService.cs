using System;
using System.Collections.Generic;
using FishNet.Connection;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network;
using WelwiseItemInShopModule.Server.Scripts;
using WelwiseItemInShopModule.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseEmotionsModule.Runtime.Server.Scripts.Animations.Network
{
    public class ClientsSelectedEmotionsDataProviderService : ClientsSelectedItemsDataProviderService<
        SelectedEmotionsData, SelectedEmotionData>
    {
        public ClientsSelectedEmotionsDataProviderService(EmotionsAnimationsConfig itemsConfig) : base(itemsConfig)
        {
        }
    }
}