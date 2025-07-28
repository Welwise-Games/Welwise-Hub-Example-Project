using System;
using System.Collections.Generic;
using System.Linq;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network;
using WelwiseItemInShopModule.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseEmotionsModule.Runtime.Shared.Scripts
{
    [Serializable]
    public class SelectedEmotionsData : IClientSelectedItemsData<SelectedEmotionData>
    {
        public List<SelectedEmotionData> SelectedItemsData { get; set; }

        public SelectedEmotionsData(List<SelectedEmotionData> selectedItemsData,
            EmotionsAnimationsConfig emotionsAnimationsConfig)
        {
            SelectedItemsData = Enumerable.Range(0, emotionsAnimationsConfig.MaxSelectedItemsNumber).Select(i =>
            {
                var selectedItemData = selectedItemsData.SafeGet(i);
                return new SelectedEmotionData(i, selectedItemData == null ? i.ToString() : selectedItemData.Index);
            }).ToList();
        }

        public SelectedEmotionsData(EmotionsAnimationsConfig emotionsAnimationsConfig)
        {
            SelectedItemsData = Enumerable.Range(0, emotionsAnimationsConfig.MaxSelectedItemsNumber).Select(i =>
                new SelectedEmotionData(i, emotionsAnimationsConfig.Configs.SafeGet(i)?.Index)).ToList();
        }

        public SelectedEmotionsData()
        {
        }
    }
}