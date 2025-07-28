using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseSharedModule.Runtime.Client.Scripts.Localization;
using WelwiseSharedModule.Runtime.Client.Scripts.Tools;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseUICircleItemModule.Runtime.Scripts
{
    public class ItemsCircleWindowController
    {
        public event Action<int> ButtonClicked;

        private readonly ItemsCircleWindow _window;
        private readonly Color _startButtonsColor;

        private readonly Func<string, UniTask<string>> _getTargetItemNameTextAsyncFunc;
        private readonly Func<UniTask<string>> _getTargetItemDefaultNameTextAsyncFunc;
        private readonly Func<int, string> _getItemIndexByButtonIndexFunc;
        private readonly Func<string, Sprite> _getItemSpriteByItemIndex;
        private readonly Func<string, string> _getItemNameByItemIndex;
        private readonly Func<KeyCode[]> _openCircleKeycodesFunc;

        public ItemsCircleWindowController(ItemsCircleWindow window, Func<bool> canSwitchingPopupOpenStateFunc,
            Func<bool> canDisableCursorOnCloseFunc, Func<string, UniTask<string>> getTargetItemNameTextAsyncFunc,
            Func<UniTask<string>> getTargetItemDefaultNameTextAsyncFunc, Func<KeyCode[]> openCircleKeycodesFunc,
            Func<int, string> getItemIndexByButtonIndexFunc, Func<string, Sprite> getItemSpriteByItemIndex, Func<string, string> getItemNameByItemIndex)
        {
            _window = window;
            _getTargetItemNameTextAsyncFunc = getTargetItemNameTextAsyncFunc;
            _openCircleKeycodesFunc = openCircleKeycodesFunc;
            _getItemIndexByButtonIndexFunc = getItemIndexByButtonIndexFunc;
            _getItemSpriteByItemIndex = getItemSpriteByItemIndex;
            _getItemNameByItemIndex = getItemNameByItemIndex;
            _getTargetItemDefaultNameTextAsyncFunc = getTargetItemDefaultNameTextAsyncFunc;

            _startButtonsColor = window.CircleButtons.FirstOrDefault()?.Button.image.color ?? Color.white;

            SubscribeCircleItemButtons();
            TryCreatingInputHandlerAndSubscribeOpenPopupButton(canSwitchingPopupOpenStateFunc,
                canDisableCursorOnCloseFunc);

            _window.OpenCircleKeyCodeParent.gameObject.SetActive(!DeviceDetectorTools.IsMobile());

            window.Popup.Opened += FillPopupButtonsItemImagesSprites;

            MarkItemButtonAsTarget(0);
        }

        public void FillPopupButtonItemImageSprite(int buttonIndex)
        {
            var itemIndex = _getItemIndexByButtonIndexFunc.Invoke(buttonIndex);

            var button = _window.CircleButtons.SafeGet(buttonIndex);

            if (!button)
                return;

            var isItemIndexNull = itemIndex == null;

            button.ItemImage.gameObject.SetActive(!isItemIndexNull);

            if (isItemIndexNull)
                return;

            button.ItemImage.sprite = _getItemSpriteByItemIndex.Invoke(itemIndex);
        }

        private void FillPopupButtonsItemImagesSprites()
        {
            for (var i = 0; i < _window.CircleButtons.Length; i++)
                FillPopupButtonItemImageSprite(i);
        }

        private void TryCreatingInputHandlerAndSubscribeOpenPopupButton(
            Func<bool> canSwitchingPopupOpenStateFunc, Func<bool> canDisableCursorOnCloseFunc)
        {
            _window.SetOpenStateButton.onClick.AddListener(() => _window.Popup.TrySettingOpenState());
            
            if (DeviceDetectorTools.IsMobile()) return;

            _window.OpenCircleKeyCodeText.text = _openCircleKeycodesFunc.Invoke().FirstOrDefault().ToString();
            new ItemsCircleWindowStandaloneInputHandler(_window, canSwitchingPopupOpenStateFunc,
                canDisableCursorOnCloseFunc, _openCircleKeycodesFunc);
        }

        private void SubscribeCircleItemButtons()
        {
            for (var i = 0; i < _window.CircleButtons.Length; i++)
            {
                var button = _window.CircleButtons[i];
                var buttonIndex = i;
                button.RaycastableImage.alphaHitTestMinimumThreshold = _window.CirclePartAlphaHitTestMinimumThreshold;
                button.Button.onClick.AddListener(() => ButtonClicked?.Invoke(buttonIndex));

                button.PointerEnterExitObserver.EnteredWithoutArgs += () =>
                {
                    DrawAllButtonsImagesWithDefaultColor();
                    MarkItemButtonAsTarget(buttonIndex);
                };
            }
        }

        private void MarkItemButtonAsTarget(int buttonIndex)
        {
            RotateTargetItemPointerImage(buttonIndex);
            DrawButtonImageWithTargetButtonColor(buttonIndex);
            SetTargetItemNameTextAsync(buttonIndex);
        }

        private async void SetTargetItemNameTextAsync(int buttonIndex)
        {
            var itemIndex = _getItemIndexByButtonIndexFunc.Invoke(buttonIndex);

            var itemName = itemIndex != null
                ? _getItemNameByItemIndex.Invoke(itemIndex)
                : null;

            _window.TargetItemNameText.text = 
                itemIndex == null || itemName == null
                    ? await _getTargetItemDefaultNameTextAsyncFunc.Invoke()
                    : await _getTargetItemNameTextAsyncFunc.Invoke(itemName);
        }

        private void DrawAllButtonsImagesWithDefaultColor() =>
            _window.CircleButtons.ForEach(button => button.CirclePartImage.color = _startButtonsColor);

        private void DrawButtonImageWithTargetButtonColor(int buttonIndex) =>
            _window.CircleButtons[buttonIndex].CirclePartImage.color = _window.SelectedCircleButtonColor;

        private void RotateTargetItemPointerImage(int buttonIndex)
        {
            var startRotation = _window.TargetItemPointerImage.transform.rotation.eulerAngles;
            _window.TargetItemPointerImage.transform.rotation = Quaternion.Euler(new Vector3(startRotation.x,
                startRotation.y, 180 - buttonIndex * 45));
        }
    }
}