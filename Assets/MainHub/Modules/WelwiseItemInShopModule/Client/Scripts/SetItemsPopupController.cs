using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WelwiseItemInShopModule.Shared.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts.Localization;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseItemInShopModule.Client.Scripts
{
    public class SetItemsPopupController<TSelectedItemData, TClientSelectedItemsData>
        where TSelectedItemData : class, ISelectedItemData
        where TClientSelectedItemsData : IClientSelectedItemsData<TSelectedItemData>
    {
        public event Action ChangedButtonSelectedMode;

        public readonly SetItemsPopup SetItemsPopup;
        public readonly SetItemsModel<TSelectedItemData, TClientSelectedItemsData> SetItemsModel;

        private readonly List<SetItemButtonController<TSelectedItemData, TClientSelectedItemsData>>
            _buttonsControllers =
                new List<SetItemButtonController<TSelectedItemData, TClientSelectedItemsData>>();

        private readonly IItemsConfig<IIndexableItemConfig> _itemsConfig;
        private readonly IItemsViewConfig<IItemViewConfig> _itemsViewConfig;

        private readonly
            ISetItemsUIFactory<TSelectedItemData, TClientSelectedItemsData,
                SetItemButtonController<TSelectedItemData, TClientSelectedItemsData>>
            _setItemsUIFactory;

        private readonly Transform _buttonsParent;
        private readonly ErrorTextController _errorTextController;

        private readonly string _setItemsLocalizationTableName;
        private readonly string _maximumIsNItemsLocalizationKeyName;
        private readonly float _scaleMultiplierOnBecomeTarget;
        private readonly float _speedChangingScaleOnSetTargetState;


        public SetItemsPopupController(IItemsConfig<IIndexableItemConfig> itemsConfig,
            IItemsViewConfig<IItemViewConfig> itemsViewConfig,
            ISetItemsUIFactory<TSelectedItemData, TClientSelectedItemsData,
                SetItemButtonController<TSelectedItemData, TClientSelectedItemsData>> setItemsUiFactory,
            SetItemsPopup popup, Transform buttonsParent,
            SetItemsModel<TSelectedItemData, TClientSelectedItemsData> setItemsModel,
            string setItemsLocalizationTableName,
            string maximumIsNItemsLocalizationKeyName, float scaleMultiplierOnBecomeTarget, float speedChangingScaleOnSetTargetState)
        {
            SetItemsPopup = popup;
            SetItemsModel = setItemsModel;
            _setItemsLocalizationTableName = setItemsLocalizationTableName;
            _maximumIsNItemsLocalizationKeyName = maximumIsNItemsLocalizationKeyName;
            _scaleMultiplierOnBecomeTarget = scaleMultiplierOnBecomeTarget;
            _speedChangingScaleOnSetTargetState = speedChangingScaleOnSetTargetState;
            _itemsConfig = itemsConfig;
            _itemsViewConfig = itemsViewConfig;
            _setItemsUIFactory = setItemsUiFactory;
            _errorTextController =
                new ErrorTextController(popup.TooMuchSelectedItemsText, itemsViewConfig.ErrorTextConfig);
            _buttonsParent = buttonsParent;

            popup.ClearSelectedItems.onClick.AddListener(MakeAllButtonsUnselected);
        }

        public void InitializeOnOpen(bool shouldShowButton)
        {
            SetItemsModel.RevertToSavedClientItemsData();
            RecreateButtons();
            SetItemsPopup.Popup.TryOpening();
            SetItemsPopup.ClearSelectedItems.gameObject.SetActive(shouldShowButton);
        }

        public void DeInitializeOnClose() => DestroyAllButtons();

        private void MakeAllButtonsUnselected() =>
            _buttonsControllers.ForEach(button =>
                button.TrySettingUnselectedModeAndUpdateView());

        public void ReturnLastSavedValuesAndUpdateView()
        {
            SetItemsModel.RevertToSavedClientItemsData();
            _buttonsControllers.ForEach(button => button.ReturnLastSavedValuesAndUpdateView());
        }

        private void DestroyAllButtons()
        {
            _buttonsControllers.ForEach(button => UnityEngine.Object.Destroy(button.ButtonView.gameObject));
            _buttonsControllers.Clear();
        }

        private void RecreateButtons()
        {
            DestroyAllButtons();
            SpawnSettingsItemsButtonsAsync();
        }

        private async void SpawnSettingsItemsButtonsAsync()
        {
            foreach (var itemConfig in _itemsConfig.Configs)
            {
                var viewConfig =
                    _itemsViewConfig.Configs.FirstOrDefault(config =>
                        config.ItemIndex == itemConfig.Index);

                if (viewConfig == null)
                    continue;

                var buttonController = await _setItemsUIFactory.GetNewSetItemButtonController(
                    _buttonsParent, viewConfig, _scaleMultiplierOnBecomeTarget, _speedChangingScaleOnSetTargetState);

                _buttonsControllers.Add(buttonController);

                buttonController.ReturnLastSavedValuesAndUpdateView();

                buttonController.ChangedSelectedMode += OnButtonChangeSelectedMode;

                buttonController.ButtonView.SetButton.onClick.AddListener(() =>
                    buttonController.TrySettingSelectedModeAndUpdateView(SetItemsModel
                        .GetFirstTemporarySelectedItemDataWithoutItem));

                buttonController.GotTooMuchItemsError += async () =>
                {
                    _errorTextController.SetTextAndStartAnimationAsync(
                        await LocalizationTools.GetLocalizedStringAsync(
                            _setItemsLocalizationTableName,
                            _maximumIsNItemsLocalizationKeyName,
                            SetItemsModel.TemporarySelectedItemsData.Count.ToString()));
                };
            }
        }


        private void OnButtonChangeSelectedMode(int? itemOrdinalIndexBeforeChange, int? newItemOrdinalIndex,
            string itemIndex)
        {
            TryUpdatingSelectedItemsOrdinalIndexes(itemOrdinalIndexBeforeChange, newItemOrdinalIndex,
                itemIndex);

            ChangedButtonSelectedMode?.Invoke();
        }

        private void TryUpdatingSelectedItemsOrdinalIndexes(int? itemOrdinalIndexBeforeChange,
            int? newItemOrdinalIndex, string itemIndex)
        {
            if (!itemOrdinalIndexBeforeChange.HasValue && !newItemOrdinalIndex.HasValue)
                return;

            SetItemsModel.UpdateSelectedItemData(
                itemOrdinalIndexBeforeChange ?? newItemOrdinalIndex.Value,
                itemIndex, newItemOrdinalIndex.HasValue);
        }
    }
}