using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseClothesSharedModule.Runtime.Client.Scripts
{
    public class ColorableClothesViewController
    {
        private readonly Dictionary<ItemCategory, ClothesSkinnedMeshRendererController>
            _clothesSkinnedMeshRendererControllerByItemCategory;

        private readonly Dictionary<ItemViewConfig, ColorableClothesSerializableComponents>
            _clothesInstancesComponentsByConfig = new Dictionary<ItemViewConfig, ColorableClothesSerializableComponents>();

        private readonly ItemsViewConfig _itemsViewConfig;

        public ColorableClothesViewController(EquippedItemsData equippedItemsData, ItemsViewConfig itemsViewConfig,
            ColorableClothesViewSerializableComponents colorableClothesViewSerializableComponents,
            ClothesFactory clothesFactory)
        {
            _itemsViewConfig = itemsViewConfig;

            _clothesSkinnedMeshRendererControllerByItemCategory = CollectionTools.ParseEnumToList<ItemCategory>()
                .Where(category => category is not ItemCategory.All and not ItemCategory.Color).ToDictionary(
                    category => category, category =>
                    {
                        var controller = new ClothesSkinnedMeshRendererController(
                            colorableClothesViewSerializableComponents.MainSkinnedMeshRenderer,
                            colorableClothesViewSerializableComponents.transform, clothesFactory,
                            (colorableClothesViewSerializableComponents.DefaultClothesInstances
                                .Find(instance => instance.ItemCategory == category)?.Instances)?.ToList());
                        controller.UpdatedInstance += UpdateInstancesComponentsByConfigDictionary;
                        controller.RemovedInstance += config => _clothesInstancesComponentsByConfig.Remove(config);
                        return controller;
                    });

            SetClothesInstancesByData(equippedItemsData);
        }

        public void TrySettingClothesInstance(ItemViewConfig itemConfig, ItemCategory itemCategory, bool shouldTakeOff)
        {
            _clothesSkinnedMeshRendererControllerByItemCategory.GetValueOrDefault(itemCategory)
                ?.UpdateInstanceAsync(itemConfig, shouldTakeOff);
        }

        public void TrySettingItemCategoryInstancesActiveState(ItemCategory itemCategory, bool shouldEnableInstances)
        {
            var controller = _clothesSkinnedMeshRendererControllerByItemCategory.GetValueOrDefault(itemCategory);
            
            controller?.SetShouldEnableInstances(shouldEnableInstances);
            controller?.SetActivePrefabInstances();
        }

        public Material[] GetClothesInstanceMaterials(ItemViewConfig itemConfig) =>
            _clothesInstancesComponentsByConfig.GetValueOrDefault(itemConfig)?.RendererWithMaterials.sharedMaterials;

        public void TrySettingClothesColor(ItemViewConfig itemConfig, int materialIndex, Color color)
        {
            var instance = _clothesInstancesComponentsByConfig.GetValueOrDefault(itemConfig);

            if (instance && materialIndex < instance.RendererWithMaterials.sharedMaterials.Length)
                instance.RendererWithMaterials.sharedMaterials[materialIndex].color = color;
        }

        private void UpdateInstancesComponentsByConfigDictionary(ItemViewConfig oldConfig, ItemViewConfig newConfig,
            ColorableClothesSerializableComponents colorableClothesSerializableComponents)
        {
            if (oldConfig != null)
                _clothesInstancesComponentsByConfig.Remove(oldConfig);

            _clothesInstancesComponentsByConfig.AddOrAppoint(newConfig, colorableClothesSerializableComponents);
        }

        public void SetClothesInstancesByData(EquippedItemsData equippedItemsData)
        {
            foreach (var itemData in equippedItemsData.ItemsData)
            {
                var itemConfig = itemData.ItemIndex == null ? null : _itemsViewConfig.TryGettingConfig(itemData.ItemIndex);
                var category = itemData.ItemCategory;
                TrySettingClothesInstance(itemConfig, category, false);
            }
        }
    }
}