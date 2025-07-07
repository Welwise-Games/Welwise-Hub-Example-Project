using System;
using System.Collections.Generic;
using System.Linq;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure.Services;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data;
using WelwiseSharedModule.Runtime.Client.Scripts.Interfaces;
using WelwiseSharedModule.Runtime.Client.Scripts.NetworkModule;
using WelwiseSharedModule.Runtime.Client.Scripts.Tools;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem
{
    public class ShopSettingEquippedItemsModel
    {
        public IReadOnlyList<EquippedItemData> GetTemporaryEquippedItemsData =>
            _temporaryCustomizationData.EquippedItemsData.ItemsData;

        public bool IsModified =>
            _modifiables.Any(modifiable => modifiable.IsModified) ||
            !_clientsCustomizationDataProviderService.AreCustomizationDataEqual(
                _lastClientCustomizationDataDataSnapshotOnApplyOrRevertChanges,
                _temporaryCustomizationData);

        private CustomizationData _temporaryCustomizationData;
        private CustomizationData _lastClientCustomizationDataDataSnapshotOnApplyOrRevertChanges;

        public event Action AppliedChanges, RevertedChanges;

        public event Action<string> ChangedNickname;
        public event Action<CustomizationData> ChangedPlayerCustomizationData;

        private readonly ClientsDataProviderService _clientsDataProviderService;
        private readonly ClientsCustomizationDataProviderService _clientsCustomizationDataProviderService;
        private readonly ItemsConfig _itemsConfig;

        private readonly HashSet<IModifiable> _modifiables = new HashSet<IModifiable>();


        public ShopSettingEquippedItemsModel(ItemsConfig itemsConfig,
            ClientsDataProviderService clientsDataProviderService,
            ClientsCustomizationDataProviderService clientsCustomizationDataProviderService)
        {
            _itemsConfig = itemsConfig;
            _clientsDataProviderService = clientsDataProviderService;
            _clientsCustomizationDataProviderService = clientsCustomizationDataProviderService;

            RevertLastSavedClientDataForTemporary();
        }

        public void AddModifiable(IModifiable modifiable) => _modifiables.Add(modifiable);

        public void SetName(string name) => ChangedNickname?.Invoke(name);

        public void TryRevertingChanges()
        {
            if (!IsModified)
                return;

            RevertLastSavedClientDataForTemporary();
            RevertedChanges?.Invoke();
        }

        public void TrySettingTemporaryEquippedItem(ItemConfig itemConfig, bool shouldTakeOff)
        {
            if (itemConfig == null) return;

            var currentItemDataIndex =
                _temporaryCustomizationData.EquippedItemsData.ItemsData.FindIndex(itemData =>
                    itemData.ItemCategory == itemConfig.ItemCategory);

            if (currentItemDataIndex == -1)
                return;

            var currentItemData = _temporaryCustomizationData.EquippedItemsData.ItemsData[currentItemDataIndex];
            _temporaryCustomizationData.EquippedItemsData.ItemsData[currentItemDataIndex] =
                shouldTakeOff
                    ? new EquippedItemData(null, new Dictionary<int, float>(), currentItemData.ItemCategory)
                    : new EquippedItemData(itemConfig.ItemIndex, currentItemData.ColorValuesByMaterialIndex,
                        currentItemData.ItemCategory);
        }


        public void UpdateTemporarySkinColor(float value) =>
            _temporaryCustomizationData.AppearanceData.SkinColor = value;

        public void UpdateTemporaryDefaultClothesEmissionColor(float value) =>
            _temporaryCustomizationData.AppearanceData.DefaultClothesEmissionColor = value;


        public void TrySettingTemporaryEquippedItemColor(string itemIndex, float color, int materialIndex)
        {
            var itemConfig = _itemsConfig.TryGettingConfig(itemIndex);

            if (itemConfig == null) return;

            var currentItemDataIndex =
                _temporaryCustomizationData.EquippedItemsData.ItemsData.FindIndex(itemData =>
                    _itemsConfig.TryGettingConfig(itemData.ItemIndex)?.ItemCategory == itemConfig.ItemCategory);

            if (currentItemDataIndex == -1) return;

            _temporaryCustomizationData.EquippedItemsData.ItemsData[currentItemDataIndex]
                .ColorValuesByMaterialIndex.AddOrAppoint(materialIndex, color);
        }

        public void ApplyTemporaryChanges()
        {
            _lastClientCustomizationDataDataSnapshotOnApplyOrRevertChanges =
                GetPlayerCustomizationDataSnapshot(_temporaryCustomizationData);
            ChangedPlayerCustomizationData?.Invoke(_temporaryCustomizationData);
            AppliedChanges?.Invoke();
        }

        private void RevertLastSavedClientDataForTemporary()
        {
            var ownerPlayerCustomizationData =
                _clientsCustomizationDataProviderService.GetClientPlayerCustomizationData(SharedNetworkTools
                    .OwnerConnection);

            _temporaryCustomizationData = GetPlayerCustomizationDataSnapshot(ownerPlayerCustomizationData);
            _lastClientCustomizationDataDataSnapshotOnApplyOrRevertChanges =
                GetPlayerCustomizationDataSnapshot(_temporaryCustomizationData);
        }

        private static CustomizationData GetPlayerCustomizationDataSnapshot(
            CustomizationData sourceCustomizationData) =>
            new(new ModelAppearanceData(
                    sourceCustomizationData.AppearanceData.DefaultClothesEmissionColor,
                    sourceCustomizationData.AppearanceData.SkinColor),
                new EquippedItemsData(
                    new List<EquippedItemData>(sourceCustomizationData.EquippedItemsData.ItemsData)));
    }
}