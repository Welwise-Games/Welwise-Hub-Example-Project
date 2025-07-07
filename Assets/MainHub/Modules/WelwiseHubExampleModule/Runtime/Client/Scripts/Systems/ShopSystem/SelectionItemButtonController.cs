using System;
using DG.Tweening;
using UnityEngine;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts.Localization;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem
{
    public class SelectionItemButtonController
    {
        public bool IsSelected { get; private set; }

        public event Action<bool> SetSelectedMode;

        public readonly IIndexableItemConfig TargetItemConfig;
        public readonly SelectionItemButtonView View;
        private readonly SelectionItemButtonTargetStateAnimationConfig _animationConfig;

        public 
            SelectionItemButtonController(IIndexableItemConfig itemConfig,
            SelectionItemButtonView view, SelectionItemButtonTargetStateAnimationConfig animationConfig)
        {
            View = view;
            _animationConfig = animationConfig;
            view.ItemImage.sprite = itemConfig.ItemSprite;
            TargetItemConfig = itemConfig;
            UpdateItemNameTextAsync(view, itemConfig.ItemName);

            UpdateTargetStateView(false);
            
            view.PointerEnterExitObserver.EnteredWithoutArgs += () => UpdateTargetStateView(true);
            view.PointerEnterExitObserver.ExitedWithoutArgs += () => UpdateTargetStateView(false);
        }

        private void UpdateTargetStateView(bool isTargetState)
        {
            View.ItemNameTextBackgroundImage.gameObject.SetActive(isTargetState);
            
            View.ItemImage.transform.DOScale(
                isTargetState ? _animationConfig.ScaleMultiplierOnBecomeTarget * Vector3.one : Vector3.one,
                _animationConfig.SpeedChangingScaleOnSetTargetState);
        }

        private static async void UpdateItemNameTextAsync(SelectionItemButtonView view, string name)
            => view.ItemNameText.text =
                await LocalizationTools.GetLocalizedStringAsync(LocalizationTablesHolder.Items, name);

        public void SetSelectionMode(bool isSelected, bool shouldInvokeAction = true)
        {
            View.SelectedImage.gameObject.SetActive(isSelected);
            IsSelected = isSelected;
            
            if (shouldInvokeAction)
                SetSelectedMode?.Invoke(isSelected);
        }
    }
}