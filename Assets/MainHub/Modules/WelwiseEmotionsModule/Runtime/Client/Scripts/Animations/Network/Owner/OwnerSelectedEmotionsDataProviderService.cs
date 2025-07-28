using System;
using System.Collections.Generic;
using System.Linq;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network;
using WelwiseItemInShopModule.Client.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts.Animations.Network.Owner
{
    public class OwnerSelectedEmotionsDataProviderService : OwnerSelectedItemsDataProviderService<SelectedEmotionData, SelectedEmotionsData>
    {
        public OwnerSelectedEmotionsDataProviderService(
            List<SelectedEmotionData> selectedEmotionsData) : base(selectedEmotionsData)
        {
            
        }
    }
}