using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data
{
    public class SharedClientsCustomizationDataProviderService
    {
        public event Action<NetworkConnection, CustomizationData> ChangedClientPlayerCustomizationData;

        private readonly ClientsDataProviderService _clientsDataProviderService;
        private readonly ClientsConfig _clientsConfig;
        private readonly ItemsConfig _itemsConfig;

        public SharedClientsCustomizationDataProviderService(ClientsDataProviderService clientsDataProviderService,
            ClientsConfig clientsConfig, ItemsConfig itemsConfig)
        {
            _clientsDataProviderService = clientsDataProviderService;
            _clientsConfig = clientsConfig;
            _itemsConfig = itemsConfig;
        }

        public void TrySettingClientPlayerCustomizationData(NetworkConnection networkConnection,
            CustomizationData inputCustomizationData, bool shouldCheckIsDataValid = true)
        {
            if (!CanSetClientPlayerCustomizationData(networkConnection, inputCustomizationData,
                    out var playerCustomizationData, shouldCheckIsDataValid))
                return;

            _clientsDataProviderService.Data[networkConnection].CustomizationData = inputCustomizationData;

            ChangedClientPlayerCustomizationData?.Invoke(networkConnection, inputCustomizationData);
        }

        public CustomizationData GetClientPlayerCustomizationData(NetworkConnection networkConnection)
            => _clientsDataProviderService.Data.GetValueOrDefault(networkConnection)?.CustomizationData;

        public bool CanSetClientPlayerCustomizationData(NetworkConnection networkConnection,
            CustomizationData customizationData, out CustomizationData currentCustomizationData,
            bool shouldCheckIsDataValid = true)
        {
            currentCustomizationData = GetClientPlayerCustomizationData(networkConnection);

            return customizationData != null && (!shouldCheckIsDataValid || (customizationData
                    .AppearanceData
                    .DefaultClothesEmissionColor.IsInsideRange(
                        _clientsConfig.PlayerDefaultClothesColorMinimumValue,
                        _clientsConfig.PlayerDefaultClothesColorMaximumValue)
                && customizationData.AppearanceData.SkinColor
                    .IsInsideRange(_clientsConfig.PlayerSkinColorMinimumValue,
                        _clientsConfig
                            .PlayerDefaultClothesColorMaximumValue) &&
                customizationData.EquippedItemsData.ItemsData
                    .All(data => data.ItemIndex == null ||
                                 _itemsConfig.TryGettingConfig(data.ItemIndex) !=
                                 null) &&
                !AreCustomizationDataEqual(customizationData,
                    currentCustomizationData)));
        }

        public bool AreCustomizationDataEqual(CustomizationData firstCustomizationData,
            CustomizationData secondCustomizationData) =>
            firstCustomizationData != null && secondCustomizationData != null &&
            firstCustomizationData.AppearanceData.DefaultClothesEmissionColor ==
            secondCustomizationData.AppearanceData.DefaultClothesEmissionColor
            && firstCustomizationData.AppearanceData.SkinColor ==
            secondCustomizationData.AppearanceData.SkinColor
            && AreEquippedItemsEqual(firstCustomizationData, secondCustomizationData);

        private static bool AreEquippedItemsEqual(CustomizationData firstCustomizationData,
            CustomizationData secondCustomizationData) =>
            firstCustomizationData.EquippedItemsData.ItemsData.CustomSequenceEqual(
                secondCustomizationData.EquippedItemsData.ItemsData,
                (firstDatasEquippedItemData, secondDatasEquippedItemData) =>
                    firstDatasEquippedItemData.ColorValuesByMaterialIndex.SequenceEqual(secondDatasEquippedItemData
                        .ColorValuesByMaterialIndex) &&
                    firstDatasEquippedItemData.ItemCategory == secondDatasEquippedItemData.ItemCategory &&
                    firstDatasEquippedItemData.ItemIndex == secondDatasEquippedItemData.ItemIndex);
    }
}