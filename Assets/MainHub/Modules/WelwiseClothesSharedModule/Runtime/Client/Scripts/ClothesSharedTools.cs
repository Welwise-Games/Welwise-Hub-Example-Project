using System.Collections.Generic;
using System.Linq;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseClothesSharedModule.Runtime.Client.Scripts
{
    public static class ClothesSharedTools
    {
        public const string EquippedItemsDataFieldNameForMetaverseSavings = "EquippedItemsData";

        public static ColorableClothesViewController GetPlayerColorableClothesViewController(
            ItemsViewConfig itemsViewConfig, ClothesFactory clothesFactory,
            ColorableClothesViewSerializableComponents serializableComponents,
            EquippedItemsData equippedItemsData) =>
            new ColorableClothesViewController(
                equippedItemsData,
                itemsViewConfig,
                serializableComponents, clothesFactory);
    }
}