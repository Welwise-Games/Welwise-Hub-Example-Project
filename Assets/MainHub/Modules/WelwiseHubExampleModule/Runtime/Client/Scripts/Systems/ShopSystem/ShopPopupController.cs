using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UI;
using WebGLMobileKeyboardModule.Runtime.Scripts;
using WelwiseChangingNicknameModule.Runtime.Client.Scripts.Services;
using WelwiseChangingNicknameModule.Runtime.Shared.Scripts;
using WelwiseClothesSharedModule.Runtime.Client.Scripts;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.UISystem;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Systems.ShopSystem;
using WelwiseSharedModule.Runtime.Client.Scripts.Localization;
using WelwiseSharedModule.Runtime.Client.Scripts.NetworkModule;
using WelwiseSharedModule.Runtime.Client.Scripts.Tools;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;
using Object = UnityEngine.Object;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem
{
    public class ShopPopupController
    {
        public event Action<ItemCategory> SelectedItemCategory;
        public event Action<ItemCategory> CreatedAllButtons;

        private ItemCategory _targetItemCategory = ItemCategory.All;
        private bool _isAnyCategorySelected;

        public readonly ShopPopup ShopPopup;
        public readonly ShopSettingEquippedItemsModel ShopSettingEquippedItemsModel;
        private readonly ItemsConfig _itemsConfig;
        private readonly ItemsViewConfig _itemsViewConfig;
        private readonly ClientsDataProviderService _clientsDataProviderService;
        private readonly ShopUIFactory _shopUIFactory;
        private readonly SharedClientsNicknamesConfig _sharedClientsNicknamesConfig;
        private readonly ColorableClothesViewController _previewColorableClothesViewController;


        private readonly List<SelectionItemButtonController> _targetCategoryButtonsControllers =
            new List<SelectionItemButtonController>();

        private readonly ItemsColoringPopupController _itemsColoringPopupController;


        public ShopPopupController(ShopSettingEquippedItemsModel shopSettingEquippedItemsModel,
            ShopPopup shopPopup, ItemsConfig itemsConfig, ShopUIFactory shopUIFactory,
            ClientsDataProviderService clientsDataProviderService,
            ClientsNicknamesProviderService clientsNicknamesProviderService,
            SkinColorChangerController playerPreviewSkinColorChangerController,
            ColorableClothesViewController previewColorableClothesViewController,
            SharedClientsNicknamesConfig sharedClientsNicknamesConfig, ItemsViewConfig itemsViewConfig)
        {
            ShopSettingEquippedItemsModel = shopSettingEquippedItemsModel;
            ShopPopup = shopPopup;
            _itemsConfig = itemsConfig;
            _shopUIFactory = shopUIFactory;
            _clientsDataProviderService = clientsDataProviderService;
            _previewColorableClothesViewController = previewColorableClothesViewController;
            _sharedClientsNicknamesConfig = sharedClientsNicknamesConfig;
            _itemsViewConfig = itemsViewConfig;

            _itemsColoringPopupController = new ItemsColoringPopupController(
                previewColorableClothesViewController, itemsConfig, clientsDataProviderService,
                shopSettingEquippedItemsModel, shopPopup.ChangingItemsColorPopup,
                playerPreviewSkinColorChangerController, itemsViewConfig);

            SubscribeChangingWarningPopup();
            SubscribeChangingNamePopup();

            ShopPopup.RevertChangesButton.onClick.AddListener(() =>
                ShopSettingEquippedItemsModel.TryRevertingChanges());

            ShopPopup.SelectionItemCategoryButtons.ForEach(button =>
                button.Button.onClick.AddListener(() => TrySelectingCategoryAndUpdateViewAsync(button.ItemCategory)));

            clientsNicknamesProviderService.ChangedClientNickname += (networkConnection, _) =>
            {
                if (networkConnection.IsOwners())
                    TryFillingPlayerNameTextsByData();
            };

            ShopPopup.Popup.Opened += () => TrySelectingCategoryAndUpdateViewAsync(_targetItemCategory);

            ShopPopup.ChangingNicknamePopup.OnlyLatinLettersText.gameObject
                .SubscribeToLocalizationAndSubscribeOnDestroy(_ => UpdateOnlyLatinLettersWarningTextAsync());
        }

        private void SubscribeChangingNamePopup()
        {
            ShopPopup.ChangingNicknamePopup.ClosePopupButton.onClick.AddListener(() =>
                ShopPopup.ChangingNicknamePopup.Popup.TryClosing());
            ShopPopup.ChangingNicknamePopup.OpenPopupButton.onClick.AddListener(() =>
                ShopPopup.ChangingNicknamePopup.Popup.TryOpening());

            ShopPopup.ChangingNicknamePopup.ApplyButton.onClick.AddListener(() =>
                ShopSettingEquippedItemsModel.SetName(ShopPopup.ChangingNicknamePopup.PlayerNicknameInputField.text));

#if UNITY_WEBGL && !UNITY_EDITOR

            if (DeviceDetectorTools.IsMobile())
            {
                ShopPopup.ChangingNicknamePopup.PlayerNicknameInputField.InitializeInputFieldForMobileKeyboard(false, true,
                    "white", "rgba(0, 0, 0, 0.3)", border:"1px solid white", width: "50%", top: "25px");   
            }
#endif

            ShopPopup.ChangingNicknamePopup.CancelButton.onClick.AddListener(TryFillingPlayerNameTextsByData);
        }

        private void SubscribeChangingWarningPopup()
        {
            ShopPopup.ChangesWarningPopup.CancelButton.onClick.AddListener(() =>
                ShopPopup.ChangesWarningPopup.Popup.TryClosing());

            ShopSettingEquippedItemsModel.RevertedChanges += OnRevertChanges;
            ShopSettingEquippedItemsModel.AppliedChanges += ShopPopup.ChangesWarningPopup.Popup.TryClosing;

            ShopPopup.ApplyChangesButton.onClick.AddListener(() =>
                TryInitializingAndOpeningChangesCancellationPopupAsync(true));
            ShopPopup.CloseShopPopupButton.onClick.AddListener(TryClosingPopupOrOpenChangesWarningWindow);
        }

        private void TryClosingPopupOrOpenChangesWarningWindow()
        {
            TryInitializingAndOpeningChangesCancellationPopupAsync(false, ShopPopup.Popup.TryClosing);
        }

        private async void TryInitializingAndOpeningChangesCancellationPopupAsync(bool shouldApplyChanges,
            Action notModified = null)
        {
            var successfully = ShopSettingEquippedItemsModel.IsModified;

            if (!successfully)
            {
                notModified?.Invoke();
                return;
            }

            ShopPopup.ChangesWarningPopup.DescriptionText.text = await LocalizationTools.GetLocalizedStringAsync(
                LocalizationTablesHolder.ShopPopup,
                shouldApplyChanges
                    ? LocalizationShopKeysHolder.DoYouWantToApplyChanges
                    : LocalizationShopKeysHolder.AreYouSureToExit);

            ShopPopup.ChangesWarningPopup.HeaderText.text = await LocalizationTools.GetLocalizedStringAsync(
                LocalizationTablesHolder.ShopPopup,
                shouldApplyChanges
                    ? LocalizationShopKeysHolder.ApplyChanges
                    : LocalizationShopKeysHolder.ExitFromShop);

            ShopPopup.ChangesWarningPopup.ApplyButton.onClick.RemoveAllListeners();

            if (shouldApplyChanges)
                ShopPopup.ChangesWarningPopup.ApplyButton.onClick.AddListener(ShopSettingEquippedItemsModel
                    .ApplyTemporaryChanges);
            else
            {
                ShopPopup.ChangesWarningPopup.ApplyButton.onClick.AddListener(() =>
                    ShopSettingEquippedItemsModel.TryRevertingChanges());
            }

            ShopPopup.ChangesWarningPopup.ApplyButton.onClick.AddListener(() => ShopPopup.Popup.TryClosing());

            ShopPopup.ChangesWarningPopup.Popup.TryOpening();
        }

        private async UniTask RecreateButtonsAsync()
        {
            DestroyAllButtons();
            await CreateAndInitializeSelectionItemsButtonsAsync(_targetItemCategory);
        }

        private void TrySettingTemporaryEquippedItem(SelectionItemButtonController buttonController, bool shouldTakeOff)
        {
            var itemViewConfig = buttonController.TargetItemConfig as ItemViewConfig;

            var itemConfig = _itemsConfig.TryGettingConfig(itemViewConfig?.ItemIndex);

            if (itemViewConfig == null || itemConfig == null)
                return;
            
            ShopSettingEquippedItemsModel.TrySettingTemporaryEquippedItem(
                itemConfig, shouldTakeOff);

            _previewColorableClothesViewController.TrySettingClothesInstance(
                itemViewConfig, itemConfig.ItemCategory, shouldTakeOff);
        }

        private async UniTask CreateAndInitializeSelectionItemsButtonsAsync(ItemCategory itemCategory)
        {
            var targetCategoryItemsConfigs = itemCategory == ItemCategory.Color
                ? await _itemsColoringPopupController.GetEquippedItemsWithValidMaterialsAsync()
                : _itemsViewConfig.Items
                    .Where(config => itemCategory == ItemCategory.All || _itemsConfig.TryGettingConfig(config.ItemIndex)?.ItemCategory == itemCategory)
                    .Cast<IIndexableItemConfig>().ToArray();

            foreach (var itemConfig in targetCategoryItemsConfigs)
            {
                var buttonController = await _shopUIFactory.GetSelectionItemButtonControllerAsync(
                    itemCategory == ItemCategory.Color
                        ? ShopPopup.ChangingItemsColorPopup.SelectionItemButtonsParent
                        : ShopPopup.SelectionItemButtonsParent, itemConfig,
                    ShopPopup.SelectionItemButtonTargetStateAnimationConfig);

                if (itemCategory == ItemCategory.Color)
                {
                    _itemsColoringPopupController.InitializeSelectionButtonController(buttonController);
                }
                else
                {
                    InitializeNotColorSelectionItemButtonController(buttonController);
                }
            }

            CreatedAllButtons?.Invoke(_targetItemCategory);
        }

        private void InitializeNotColorSelectionItemButtonController(SelectionItemButtonController buttonController)
        {
            SetSelectionNotColorButtonMode(buttonController, false);

            buttonController.SetSelectedMode +=
                isSelected => TrySettingTemporaryEquippedItem(buttonController, !isSelected);

            buttonController.View.Button.onClick.AddListener(() =>
                SetSelectionNotColorButtonMode(buttonController, true));

            _targetCategoryButtonsControllers.Add(buttonController);
        }

        private void SetButtonsSelectionMode() =>
            _targetCategoryButtonsControllers.ForEach(controller => SetSelectionNotColorButtonMode(controller, false));

        private void SetSelectionNotColorButtonMode(SelectionItemButtonController buttonController,
            bool shouldSetToOpposite)
        {
            var isSelected = ShopSettingEquippedItemsModel.GetTemporaryEquippedItemsData
                .Any(equippedItemData =>
                    equippedItemData.ItemIndex == buttonController.TargetItemConfig.ItemIndex);

            isSelected = shouldSetToOpposite ? !isSelected : isSelected;

            if (isSelected)
            {
                var targetItemConfig = _itemsConfig.TryGettingConfig(buttonController.TargetItemConfig.ItemIndex).ItemCategory;
                DeselectNotColorButtonsByCategory(targetItemConfig);
            }

            buttonController.SetSelectionMode(isSelected);
        }

        private void DeselectNotColorButtonsByCategory(ItemCategory targetItemConfig)
        {
            _targetCategoryButtonsControllers
                .Where(button =>
                    button.TargetItemConfig is ItemViewConfig itemViewConfig && _itemsConfig.TryGettingConfig(itemViewConfig.ItemIndex)?.ItemCategory == targetItemConfig)
                .ForEach(button => button.SetSelectionMode(false, false));
        }

        private void DestroyAllButtons()
        {
            _targetCategoryButtonsControllers.ForEach(button => Object.Destroy(button.View.gameObject));
            _targetCategoryButtonsControllers.Clear();
            _itemsColoringPopupController.DestroyAllButtons();
        }

        private void OnRevertChanges()
        {
            ShopPopup.ChangesWarningPopup.Popup.TryClosing();
            TryFillingPlayerNameTextsByData();
            SetButtonsSelectionMode();
            _itemsColoringPopupController.TryRevertingSliderValueAndSelectionMaterialButtonsColorsByData();
            SetClothesInstanceByData();
        }

        private async void TrySelectingCategoryAndUpdateViewAsync(ItemCategory itemCategory)
        {
            switch (_isAnyCategorySelected)
            {
                case true when itemCategory == _targetItemCategory:
                    return;
                case false:
                    UpdateOnlyLatinLettersWarningTextAsync();
                    SetClothesInstanceByData();
                    LayoutRebuilder.ForceRebuildLayoutImmediate(ShopPopup.SelectionItemCategoryButtonsParent);
                    break;
            }

            _isAnyCategorySelected = true;
            _targetItemCategory = itemCategory;

            ShopPopup.SelectionTargetItemCategoryImageParent.DOMove(ShopPopup.SelectionItemCategoryButtons
                    .First(button => button.ItemCategory == itemCategory).transform.position,
                _isAnyCategorySelected ? ShopPopup.SelectionTargetItemCategoryImageAnimationDuration : 0);

            TryFillingPlayerNameTextsByData();

            ShopPopup.ChangingSelectionItemsPopup.Popup.TrySettingOpenState(itemCategory != ItemCategory.Color);
            ShopPopup.ChangingItemsColorPopup.Popup.TrySettingOpenState(itemCategory == ItemCategory.Color);

            SelectedItemCategory?.Invoke(itemCategory);

            await RecreateButtonsAsync();

            if (itemCategory != ItemCategory.Color) return;

            _itemsColoringPopupController.OnSelectColorCategory();
        }

        private void SetClothesInstanceByData()
        {
            _previewColorableClothesViewController.SetClothesInstancesByData(_clientsDataProviderService.Data
                .GetOwners().CustomizationData.EquippedItemsData);
        }

        private async void UpdateOnlyLatinLettersWarningTextAsync() =>
            ShopPopup.ChangingNicknamePopup.OnlyLatinLettersText.text =
                await LocalizationTools.GetLocalizedStringAsync(LocalizationTablesHolder.ShopPopup,
                    LocalizationShopKeysHolder.OnlyLettersAndNumbers,
                    _sharedClientsNicknamesConfig.MinimalNicknameLength.ToString(),
                    _sharedClientsNicknamesConfig.MaximumNicknameLength.ToString());

        private void TryFillingPlayerNameTextsByData()
        {
            if (!ShopPopup.Popup.IsOpen)
                return;

            ShopPopup.PlayerNicknameText.text = ShopPopup.ChangingNicknamePopup.PlayerNicknameInputField.text =
                _clientsDataProviderService.Data.GetOwners().AccountData.Nickname;
        }
    }
}