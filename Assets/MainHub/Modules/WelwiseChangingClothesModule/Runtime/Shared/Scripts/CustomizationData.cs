using System;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;

namespace WelwiseChangingClothesModule.Runtime.Shared.Scripts
{
    [Serializable]
    public class CustomizationData
    {
        public ModelAppearanceData AppearanceData { get; set; }
        public EquippedItemsData EquippedItemsData { get; set; }

        public CustomizationData(ModelAppearanceData appearanceData, EquippedItemsData equippedItemsData)
        {
            AppearanceData = appearanceData;
            EquippedItemsData = equippedItemsData;
        }

        public CustomizationData()
        {
            
        }
    }
}