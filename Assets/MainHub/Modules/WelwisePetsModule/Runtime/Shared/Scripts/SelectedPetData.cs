using System;
using WelwiseItemInShopModule.Shared.Scripts;

namespace WelwisePetsModule.Runtime.Shared.Scripts
{
    [Serializable]
    public class SelectedPetData : ISelectedItemData
    {
        public string Index { get; set; }
        public int OrdinalIndex { get; set; }

        public SelectedPetData(int ordinalIndex, string index = null)
        {
            Index = index;
            OrdinalIndex = ordinalIndex;
        }
        
        public SelectedPetData() {}
    }
}