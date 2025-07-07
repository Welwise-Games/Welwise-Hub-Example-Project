using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseClothesSharedModule.Runtime.Client.Scripts;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Systems.ShopSystem;
using WelwiseSharedModule.Runtime.Client.Scripts.NetworkModule;
using WelwiseSharedModule.Runtime.Client.Scripts.Tools;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem
{
    public class ItemsColoringPopupController
    {
        private IIndexableItemConfig _targetIndexableItemConfig;
        private int _selectedMaterialIndex;
        private int _selectedChangingColorButtonIndex;

        private readonly ChangingItemsColorPopup _changingItemsColorPopup;
        private readonly ShopSettingEquippedItemsModel _shopSettingEquippedItemsModel;
        private readonly List<PersistentColorableItem> _persistentColorableItems;
        private readonly ColorableClothesViewController _previewColorableClothesViewController;
        private readonly ClientsDataProviderService _clientsDataProviderService;

        private readonly List<SelectionItemButtonController> _selectionItemsButtonsControllers =
            new List<SelectionItemButtonController>();

        private readonly SkinColorChangerController _playerPreviewSkinColorChangerController;
        private readonly ItemsViewConfig _itemsViewConfig;

        private const string SkinColorItemIndex = "SkinColor";
        private const string DefaultClothesEmissionColorItemIndex = "DefaultClothesEmissionColor";

        public ItemsColoringPopupController(
            ColorableClothesViewController previewColorableClothesViewController,
            ItemsConfig itemsConfig, ClientsDataProviderService clientsDataProviderService,
            ShopSettingEquippedItemsModel shopSettingEquippedItemsModel,
            ChangingItemsColorPopup changingItemsColorPopup,
            SkinColorChangerController playerPreviewSkinColorChangerController, ItemsViewConfig itemsViewConfig)
        {
            _previewColorableClothesViewController = previewColorableClothesViewController;
            _clientsDataProviderService = clientsDataProviderService;
            _shopSettingEquippedItemsModel = shopSettingEquippedItemsModel;
            _changingItemsColorPopup = changingItemsColorPopup;
            _playerPreviewSkinColorChangerController = playerPreviewSkinColorChangerController;
            _itemsViewConfig = itemsViewConfig;

            _persistentColorableItems = new List<PersistentColorableItem>
            {
                new(SkinColorItemIndex, itemsConfig.PlayerSkinColorItemSprite, SkinColorItemIndex),
                new(DefaultClothesEmissionColorItemIndex, itemsConfig.PlayerDefaultClothesEmissionColorSprite,
                    DefaultClothesEmissionColorItemIndex)
            };

            _changingItemsColorPopup.ChangingColorSlider.onValueChanged.AddListener(UpdateItemColorAndUI);
        }

        public void DestroyAllButtons()
        {
            _selectionItemsButtonsControllers.ForEach(button => Object.Destroy(button.View.gameObject));
            _selectionItemsButtonsControllers.Clear();
        }

        public void OnSelectColorCategory()
        {
            SelectFirstSelectionItemButtonIfTargetIndexableConfigIsNull();
            TryRevertingSliderValueAndSelectionMaterialButtonsColorsByData();
        }

        private void SelectFirstSelectionItemButtonIfTargetIndexableConfigIsNull()
        {
            if (_targetIndexableItemConfig != null &&
                _selectionItemsButtonsControllers.Any(button => button.TargetItemConfig == _targetIndexableItemConfig))
                return;

            DeselectAllButtons();
            _selectionItemsButtonsControllers.First().SetSelectionMode(true);
        }

        public void TryRevertingSliderValueAndSelectionMaterialButtonsColorsByData()
        {
            if (_targetIndexableItemConfig == null)
                return;

            var ownerData = _clientsDataProviderService.Data.GetOwners();

            var equippedItemDataIndex = ownerData.CustomizationData.EquippedItemsData.ItemsData
                .FindIndex(data => data.ItemIndex == _targetIndexableItemConfig.ItemIndex);

            var colorValues =
                _targetIndexableItemConfig.ItemIndex is SkinColorItemIndex or DefaultClothesEmissionColorItemIndex
                    ? GetColorValuesForPlayerAppearance(ownerData)
                    : equippedItemDataIndex == -1 || !ownerData.CustomizationData.EquippedItemsData
                        .ItemsData[equippedItemDataIndex].ColorValuesByMaterialIndex.ContainsKey(_selectedMaterialIndex)
                        ? GetClothesDefaultMaterials()
                        : ownerData.CustomizationData.EquippedItemsData.ItemsData[equippedItemDataIndex]
                            .ColorValuesByMaterialIndex;

            TrySettingActiveSelectionMaterialIndexesButtons();

            _changingItemsColorPopup.ChangingColorSlider.value = colorValues[_selectedMaterialIndex];

            for (var i = 0; i < _changingItemsColorPopup.SelectionMaterialIndexButtons.Length; i++)
            {
                var button = _changingItemsColorPopup.SelectionMaterialIndexButtons[i];
                button.image.color = GetColorBySliderValue(colorValues.GetValueOrDefault(i));
            }
        }

        private Dictionary<int, float> GetColorValuesForPlayerAppearance(ClientData ownerData) =>
            new()
            {
                {
                    0,
                    _targetIndexableItemConfig.ItemIndex is SkinColorItemIndex
                        ? ownerData.CustomizationData.AppearanceData.SkinColor
                        : ownerData.CustomizationData.AppearanceData.DefaultClothesEmissionColor
                }
            };

        private Dictionary<int, float> GetClothesDefaultMaterials() =>
            _previewColorableClothesViewController.GetClothesInstanceMaterials(
                _targetIndexableItemConfig as ItemViewConfig).Select((material, index) =>
                new { index, material.color }).ToDictionary(pair => pair.index,
                pair => GetSliderValueByColor(pair.color));

        private void TrySettingActiveSelectionMaterialIndexesButtons()
        {
            if (_targetIndexableItemConfig is ItemViewConfig itemConfig)
            {
                var materials = _previewColorableClothesViewController.GetClothesInstanceMaterials(itemConfig);

                var materialsCount = materials.Length;

                _selectedMaterialIndex = Mathf.Min(materials.Length - 1, _selectedMaterialIndex);

                for (var i = 0; i < _changingItemsColorPopup.SelectionMaterialIndexButtons.Length; i++)
                {
                    var button = _changingItemsColorPopup.SelectionMaterialIndexButtons[i];
                    button.gameObject.SetActive(materialsCount > i);
                }
            }
            else
            {
                _changingItemsColorPopup.SelectionMaterialIndexButtons.ForEach(button =>
                    button.gameObject.SetActive(false));
                _changingItemsColorPopup.SelectionMaterialIndexButtons.First().gameObject.SetActive(true);
            }
        }

        private void UpdateItemColorAndUI(float colorValue)
        {
            var color = GetColorBySliderValue(colorValue);

            switch (_targetIndexableItemConfig.ItemIndex)
            {
                case SkinColorItemIndex:
                    _shopSettingEquippedItemsModel.UpdateTemporarySkinColor(colorValue);
                    _playerPreviewSkinColorChangerController.SetSkinColor(colorValue);
                    break;
                case DefaultClothesEmissionColorItemIndex:
                    _shopSettingEquippedItemsModel.UpdateTemporaryDefaultClothesEmissionColor(colorValue);
                    _playerPreviewSkinColorChangerController.SetDefaultClothesEmissionColor(colorValue);
                    break;
                default:
                    _shopSettingEquippedItemsModel.TrySettingTemporaryEquippedItemColor(
                        _targetIndexableItemConfig.ItemIndex,
                        colorValue, _selectedMaterialIndex);

                    _previewColorableClothesViewController.TrySettingClothesColor(
                        _targetIndexableItemConfig as ItemViewConfig,
                        _selectedMaterialIndex, color);
                    break;
            }

            _changingItemsColorPopup.SelectionMaterialIndexButtons[_selectedMaterialIndex].image.color =
                color;
        }

        private Color GetColorBySliderValue(float sliderValue)
        {
            var normalizedValue = sliderValue / _changingItemsColorPopup.ChangingColorSlider.maxValue;
            return Color.HSVToRGB(normalizedValue, 1, 1);
        }

        private float GetSliderValueByColor(Color color)
        {
            Color.RGBToHSV(color, out var h, out var s, out var v);
            return h * _changingItemsColorPopup.ChangingColorSlider.maxValue;
        }

        public void InitializeSelectionButtonController(SelectionItemButtonController buttonController)
        {
            buttonController.SetSelectedMode += isSelected =>
                OnSelectionButtonSelected(buttonController);

            buttonController.View.Button.onClick.AddListener(() =>
                SelectSelectionItemButtonTrueMode(buttonController));
            _selectionItemsButtonsControllers.Add(buttonController);
        }

        private void SelectSelectionItemButtonTrueMode(SelectionItemButtonController buttonController)
        {
            DeselectAllButtons();
            buttonController.SetSelectionMode(true);
        }

        private void DeselectAllButtons() =>
            _selectionItemsButtonsControllers.ForEach(button => button.SetSelectionMode(false, false));

        private static bool IsValidMaterials(Material[] materials) =>
            materials is { Length: > 0 } &&
            materials.All(material => material);

        private void SetTargetIndexableItemConfig(SelectionItemButtonController buttonController) =>
            _targetIndexableItemConfig = buttonController.TargetItemConfig;

        private void OnSelectionButtonSelected(SelectionItemButtonController buttonController)
        {
            SetTargetIndexableItemConfig(buttonController);
            TryRevertingSliderValueAndSelectionMaterialButtonsColorsByData();
        }

        public async UniTask<IIndexableItemConfig[]> GetEquippedItemsWithValidMaterialsAsync()
        {
            /*var equippedItemsClothesSerializableComponents = _itemsConfig.Items.Select(async itemConfig =>
            {
                var isConfigItemEquipped = _clientsDataProviderService.Data.GetOwners().PlayerCustomizationData.EquippedItemsData.ItemsData
                    .Any(itemData => itemData.ItemIndex == itemConfig.ItemIndex);

                if (!isConfigItemEquipped)
                    return null;

                var colorableComponents = await AssetProvider.LoadAsync<ColorableClothesSerializableComponents>(
                    await itemConfig.PrefabReference.GetAssetIdAsync());

                return IsValidMaterials(colorableComponents?.RendererWithMaterials.sharedMaterials)
                    ? itemConfig
                    : null;
            });

            return _persistentColorableItems
                .Concat<IIndexableItemConfig>(
                    (await UniTask.WhenAll(equippedItemsClothesSerializableComponents)).Where(components =>
                        components != null))
                .ToArray<IIndexableItemConfig>();*/

            return _persistentColorableItems.ToArray();
        }
    }
}