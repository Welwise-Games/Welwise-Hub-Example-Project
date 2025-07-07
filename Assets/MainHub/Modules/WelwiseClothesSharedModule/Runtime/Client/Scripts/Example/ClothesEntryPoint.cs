using UnityEngine;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseClothesSharedModule.Runtime.Client.Scripts.Example
{
    public class ClothesEntryPoint
    {
        private readonly ItemsViewConfigsProviderService _itemsConfigsProviderService;
        private readonly ClothesFactory _clothesFactory;

        public ClothesEntryPoint(ItemsViewConfigsProviderService itemsConfigsProviderService, IAssetLoader assetLoader)
        {
            _itemsConfigsProviderService = itemsConfigsProviderService;
            _clothesFactory = new ClothesFactory(assetLoader);
        }

        public async void OnCreatePlayerAsync(
            ColorableClothesViewSerializableComponents colorableClothesViewSerializableComponents,
            EquippedItemsData equippedItemsData)
        {
            ClothesSharedTools.GetPlayerColorableClothesViewController(
                await _itemsConfigsProviderService.GetItemsViewConfigAsync(), _clothesFactory,
                colorableClothesViewSerializableComponents, equippedItemsData);
        }
    }
}