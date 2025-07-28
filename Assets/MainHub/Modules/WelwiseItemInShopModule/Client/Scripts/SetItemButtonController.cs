using System;
using DG.Tweening;
using UnityEngine;
using WelwiseItemInShopModule.Shared.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts.Localization;

namespace WelwiseItemInShopModule.Client.Scripts
{
    public class SetItemButtonController<TSelectedItemData, TClientSelectedItemsData>
        where TSelectedItemData : class, ISelectedItemData
        where TClientSelectedItemsData : IClientSelectedItemsData<TSelectedItemData>
    {
        private bool IsSelected => TargetItemOrdinalIndex.HasValue;
        public int? TargetItemOrdinalIndex { get; private set; }

        public event Action GotTooMuchItemsError;
        public event Action<int?, int?, string> ChangedSelectedMode;

        private int? _lastSavedTargetItemOrdinalIndex;
        public readonly SetItemButtonView ButtonView;

        private readonly IItemViewConfig _targetItemViewConfig;

        private readonly OwnerSelectedItemsDataProviderService<TSelectedItemData, TClientSelectedItemsData>
            _ownerSelectedPetsDataProvider;

        private readonly float _scaleMultiplierOnBecomeTarget;
        private readonly float _speedChangingScaleOnSetTargetState;
        private readonly string _itemsLocalizationTableName;

        public SetItemButtonController(SetItemButtonView buttonView, IItemViewConfig targetItemViewConfig,
            OwnerSelectedItemsDataProviderService<TSelectedItemData, TClientSelectedItemsData>
                ownerSelectedPetsDataProvider,
            float scaleMultiplierOnBecomeTarget, float speedChangingScaleOnSetTargetState, string itemsLocalizationTableName)
        {
            ButtonView = buttonView;
            _targetItemViewConfig = targetItemViewConfig;
            _ownerSelectedPetsDataProvider = ownerSelectedPetsDataProvider;
            _scaleMultiplierOnBecomeTarget = scaleMultiplierOnBecomeTarget;
            _speedChangingScaleOnSetTargetState = speedChangingScaleOnSetTargetState;
            _itemsLocalizationTableName = itemsLocalizationTableName;

            buttonView.ItemViewImage.sprite = targetItemViewConfig.Sprite;

            ChangedSelectedMode += (_, _, _) => UpdateViewAsync();

            UpdateTargetStateView(false);

            ButtonView.PointerEnterExitObserver.EnteredWithoutArgs += () => UpdateTargetStateView(true);
            ButtonView.PointerEnterExitObserver.ExitedWithoutArgs += () => UpdateTargetStateView(false);
        }

        public void ReturnLastSavedValuesAndUpdateView()
        {
            _lastSavedTargetItemOrdinalIndex = _ownerSelectedPetsDataProvider
                .GetSelectedDataByItemIndex(_targetItemViewConfig.ItemIndex)
                ?.OrdinalIndex;
            TargetItemOrdinalIndex = _lastSavedTargetItemOrdinalIndex;
            UpdateViewAsync();
        }

        public void TrySettingUnselectedModeAndUpdateView()
        {
            if (!IsSelected)
                return;

            var ordinalIndexBeforeChange = TargetItemOrdinalIndex;

            TargetItemOrdinalIndex = null;

            ChangedSelectedMode?.Invoke(ordinalIndexBeforeChange, TargetItemOrdinalIndex,
                _targetItemViewConfig.ItemIndex);
        }

        public void TrySettingSelectedModeAndUpdateView(
            Func<TSelectedItemData> getLocalFirstSelectedDataWithoutItem)
        {
            var ordinalIndexBeforeChange = TargetItemOrdinalIndex;

            if (!IsSelected)
            {
                var newData = getLocalFirstSelectedDataWithoutItem?.Invoke();

                if (newData == null)
                {
                    GotTooMuchItemsError?.Invoke();
                    return;
                }

                TargetItemOrdinalIndex = newData.OrdinalIndex;
            }
            else
            {
                TargetItemOrdinalIndex = null;
            }


            ChangedSelectedMode?.Invoke(ordinalIndexBeforeChange, TargetItemOrdinalIndex,
                _targetItemViewConfig.ItemIndex);
        }

        private async void UpdateViewAsync()
        {
            ButtonView.ItemOrdinalIndexBackgroundImage.gameObject.SetActive(IsSelected);

            if (IsSelected)
                ButtonView.ItemOrdinalIndexText.text = (TargetItemOrdinalIndex + 1).ToString();

            ButtonView.ItemNameText.text = await LocalizationTools.GetLocalizedStringAsync(
                _itemsLocalizationTableName,
                _targetItemViewConfig.ItemIndex);
        }

        private void UpdateTargetStateView(bool isTargetState)
        {
            ButtonView.ItemNameTextBackgroundImage.gameObject.SetActive(isTargetState);

            ButtonView.ItemViewImage.transform.DOScale(
                isTargetState ? _scaleMultiplierOnBecomeTarget * Vector3.one : Vector3.one,
                _speedChangingScaleOnSetTargetState);
        }
    }
}