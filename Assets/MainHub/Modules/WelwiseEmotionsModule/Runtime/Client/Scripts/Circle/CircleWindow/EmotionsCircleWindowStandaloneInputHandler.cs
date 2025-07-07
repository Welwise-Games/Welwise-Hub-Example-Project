using System;
using System.Linq;
using UnityEngine;
using WelwiseSharedModule.Runtime.Client.Scripts.Tools;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts.Circle.CircleWindow
{
    public class EmotionsCircleWindowStandaloneInputHandler
    {
        private KeyCode _pressedButtonKeyCode;
        private readonly EmotionsCircleWindow _emotionsCircleWindow;
        private readonly EmotionsViewConfig _emotionsViewConfig;
        private readonly Func<bool> _canDisableCursorOnCloseFunc;
        private readonly Func<bool> _canSwitchingPopupOpenStateFunc;

        public EmotionsCircleWindowStandaloneInputHandler(EmotionsCircleWindow emotionsCircleWindow,
            EmotionsViewConfig emotionsViewConfig, Func<bool> canSwitchingPopupOpenStateFunc, Func<bool> canDisableCursorOnCloseFunc)
        {
            _emotionsCircleWindow = emotionsCircleWindow;
            _emotionsViewConfig = emotionsViewConfig;
            _canSwitchingPopupOpenStateFunc = canSwitchingPopupOpenStateFunc;
            _canDisableCursorOnCloseFunc = canDisableCursorOnCloseFunc;

            emotionsCircleWindow.MonoBehaviourObserver.Updated += TrySwitchingPopupOpenState;
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
            if (!_emotionsCircleWindow.Popup.IsOpen || !Input.GetKeyUp(_pressedButtonKeyCode)) return;

            if (_canDisableCursorOnCloseFunc.Invoke())
                CursorSwitcherTools.TryDisablingCursor();

            var pressedObjects = UITools.GetPointerEventRaycastResults().Select(result => result.gameObject).ToList();

            var pressedButton =
                _emotionsCircleWindow.PlayEmotionButtons.FirstOrDefault(button =>
                    pressedObjects.Contains(button.Button.targetGraphic.gameObject));

            if (pressedButton)
                pressedButton.Button.onClick?.Invoke();

            _emotionsCircleWindow.Popup.TryClosing();
            _pressedButtonKeyCode = KeyCode.None;
        }

        private void TryOpeningPopup(out bool successfully)
        {
            successfully = false;

            if (_emotionsCircleWindow.Popup.IsOpen || !_canSwitchingPopupOpenStateFunc.Invoke())
                return;

            _pressedButtonKeyCode = _emotionsViewConfig.OpenCircleKeycodes.Find(Input.GetKeyDown);

            if (_pressedButtonKeyCode == KeyCode.None) return;

            successfully = true;
            CursorSwitcherTools.TryEnablingCursor();
            _emotionsCircleWindow.Popup.TryOpening();
        }
    }
}