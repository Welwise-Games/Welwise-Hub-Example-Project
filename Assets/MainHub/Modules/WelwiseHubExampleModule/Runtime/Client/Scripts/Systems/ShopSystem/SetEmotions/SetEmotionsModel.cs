using System;
using System.Collections.Generic;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations.Network.Owner;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network;
using WelwiseItemInShopModule.Client.Scripts;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem.SetEmotions
{
    public class SetEmotionsModel : SetItemsModel<SelectedEmotionData, SelectedEmotionsData>
    {
        public SetEmotionsModel(OwnerSelectedEmotionsDataProviderService
                ownerSelectedItemsDataProviderService,
            Func<IReadOnlyList<SelectedEmotionData>, SelectedEmotionsData> getClientSelectedItemsData,
            Func<int, string, SelectedEmotionData> getSelectedItemDataFunc) : base(
            ownerSelectedItemsDataProviderService, getClientSelectedItemsData, getSelectedItemDataFunc)
        {
        }
    }
}