using System.Collections.Generic;
using System.Linq;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace MainHub.Modules.WelwiseLoadingClothesModule.Runtime.Shared.Scripts
{
    public static class ClothesLoadingTools
    {
        public static EquippedItemsData GetDefaultEquippedItemsData() =>
            new(
                CollectionTools.ToList<ItemCategory>()
                    .Where(category => category is not ItemCategory.All and not ItemCategory.Color)
                    .Select(category => new EquippedItemData(null, new Dictionary<int, float>(), category))
                    .ToList());
    }
}