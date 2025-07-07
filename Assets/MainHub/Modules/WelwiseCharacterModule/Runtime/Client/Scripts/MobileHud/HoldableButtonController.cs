using System;
using UnityEngine.EventSystems;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.MobileHud
{
    public class HoldableButtonController
    {
        private bool _isButtonDown;
        private bool _wasPressed;

        private event Action<bool> _invokedIsHold;

        public HoldableButtonController(PointerUpDownObserver pointerUpDownObserver)
        {
            pointerUpDownObserver.PointerDowned += OnPointerDown;
            pointerUpDownObserver.PointerUpped += OnPointerUp;

            _invokedIsHold += TryMakingWasPressedFalse;
        }

        public bool IsHold()
        {
            var isHold = _isButtonDown && !_wasPressed;
            _invokedIsHold?.Invoke(isHold);
            return isHold;
        }

        private void OnPointerDown(PointerEventData eventData)
        {
            _isButtonDown = true;
            _wasPressed = false;
        }

        private void OnPointerUp(PointerEventData eventData) => _isButtonDown = false;

        private void TryMakingWasPressedFalse(bool isHold)
        {
            if (isHold)
                _wasPressed = true;
        }
    }
}