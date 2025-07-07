using System;
using System.Collections.Generic;

namespace WelwiseClothesSharedModule.Runtime.Shared.Scripts
{
    [Serializable]
    public class EquippedItemsData
    {
        public List<EquippedItemData> ItemsData { get; set; }

        public EquippedItemsData() {}
        public EquippedItemsData(List<EquippedItemData> itemsData) => ItemsData = itemsData;
    }
}