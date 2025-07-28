using System;
using System.Linq;
using UnityEngine;
using WelwiseSharedModule.Runtime.Client.Scripts.Tools;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseUICircleItemModule.Runtime.Scripts
{
    public class ItemsCircleWindowStandaloneInputHandler
    {
        private KeyCode _pressedButtonKeyCode;
        private readonly ItemsCircleWindow _itemsCircleWindow;
        private readonly Func<bool> _canDisableCursorOnCloseFunc;
        private readonly Func<bool> _canSwitchingPopupOpenStateFunc;
        private readonly Func<KeyCode[]> _openCircleKeycodes;

        public ItemsCircleWindowStandaloneInputHandler(ItemsCircleWindow itemsCircleWindow,
            Func<bool> canSwitchingPopupOpenStateFunc, Func<bool> canDisableCursorOnCloseFunc, Func<KeyCode[]> openCircleKeycodes)
        {
            _itemsCircleWindow = itemsCircleWindow;
            _canSwitchingPopupOpenStateFunc = canSwitchingPopupOpenStateFunc;
            _canDisableCursorOnCloseFunc = canDisableCursorOnCloseFunc;
            _openCircleKeycodes = openCircleKeycodes;

            itemsCircleWindow.MonoBehaviourObserver.Updated += TrySwitchingPopupOpenState;
        }

        private void TrySwitchingPopupOpenState()
        {
            TryOpeningPopup(out var successfully);

            if (successfully)
                return;

            TryClosingPopup();
        }

        private void TryClosingPopup()
        {
            if (!_itemsCircleWindow.Popup.IsOpen || !Input.GetKeyUp(_pressedButtonKeyCode)) return;

            if (_canDisableCursorOnCloseFunc.Invoke())
                CursorSwitchTools.TryDisablingCursor();

            var pressedObjects = UITools.GetPointerEventRaycastResults().Select(result => result.gameObject).ToList();

            var pressedButton =
                _itemsCircleWindow.CircleButtons.FirstOrDefault(button =>
                    pressedObjects.Contains(button.Button.targetGraphic.gameObject));

            if (pressedButton)
                pressedButton.Button.onClick?.Invoke();

            _itemsCircleWindow.Popup.TryClosing();
            _pressedButtonKeyCode = KeyCode.None;
        }

        private void TryOpeningPopup(out bool successfully)
        {
            successfully = false;

            if (_itemsCircleWindow.Popup.IsOpen || !_canSwitchingPopupOpenStateFunc.Invoke())
                return;

            _pressedButtonKeyCode = _openCircleKeycodes.Invoke().FirstOrDefault(Input.GetKeyDown);

            if (_pressedButtonKeyCode == KeyCode.None) return;

            successfully = true;
            CursorSwitchTools.TryEnablingCursor();
            _itemsCircleWindow.Popup.TryOpening();
        }
    }
}