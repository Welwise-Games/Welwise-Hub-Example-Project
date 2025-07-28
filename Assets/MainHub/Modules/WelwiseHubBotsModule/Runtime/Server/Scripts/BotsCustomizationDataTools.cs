using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseHubBotsModule.Runtime.Server.Scripts
{
    public static class BotsCustomizationDataTools
    {
        public static CustomizationData GetRandomCustomizationData(
            CustomizationData currentCustomizationData, ClientsConfig clientsConfig, float setDataChance,
            ItemsConfig itemsConfig)
        {
            var appearanceData = new ModelAppearanceData(
                setDataChance.UseAsChanceAndGetResult()
                    ? Random.Range(clientsConfig.PlayerDefaultClothesColorMinimumValue,
                        clientsConfig.PlayerDefaultClothesColorMaximumValue)
                    : currentCustomizationData.AppearanceData.DefaultClothesEmissionColor,
                setDataChance.UseAsChanceAndGetResult()
                    ? Random.Range(clientsConfig.PlayerSkinColorMinimumValue,
                        clientsConfig.PlayerDefaultClothesColorMaximumValue)
                    : currentCustomizationData.AppearanceData.SkinColor);

            var equippedItemsData = new EquippedItemsData(CollectionTools.ParseEnumToList<ItemCategory>().Select(category =>
                setDataChance.UseAsChanceAndGetResult()
                    ? new EquippedItemData(
                        itemsConfig.Items.Where(item => item.ItemCategory == category).GetRandomOrDefault()?.ItemIndex,
                        new Dictionary<int, float>(), category)
                    : currentCustomizationData.EquippedItemsData.ItemsData.FirstOrDefault(data => data.ItemCategory == category)).ToList());

            return new CustomizationData(appearanceData, equippedItemsData);
        }
    }
}