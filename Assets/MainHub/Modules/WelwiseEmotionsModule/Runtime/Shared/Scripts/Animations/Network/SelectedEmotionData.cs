using System;
using WelwiseItemInShopModule.Shared.Scripts;

namespace WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network
{
    [Serializable]
    public class SelectedEmotionData : ISelectedItemData
    {
        public string Index { get; set; }
        public int OrdinalIndex { get; set; }

        public SelectedEmotionData(int ordinalIndex, string index = null)
        {
            Index = index;
            OrdinalIndex = ordinalIndex;
        }
        
        public SelectedEmotionData() {}
    }
}