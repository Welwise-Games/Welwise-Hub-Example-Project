using System;
using System.Collections.Generic;

namespace WelwiseClothesSharedModule.Runtime.Shared.Scripts
{
    [Serializable]
    public struct EquippedItemData
    {
        public ItemCategory ItemCategory { get; set; }
        public string ItemIndex { get; set; }
        public Dictionary<int, float> ColorValuesByMaterialIndex { get; set; }

        public EquippedItemData(string itemIndex, Dictionary<int, float> colorValuesByMaterialIndex, ItemCategory itemCategory)
        {
            ItemIndex = itemIndex;
            ColorValuesByMaterialIndex = colorValuesByMaterialIndex;
            ItemCategory = itemCategory;
        }
    }
}