using System.Linq;
using UnityEngine;
using WelwiseCharacterModule.Runtime.Client.Scripts.InputServices;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.MobileHud
{
    public class MobileInputService : IMobileInputService
    {
        private MobileHudController _mobileHudController;

        private readonly float _lookAreaScreenMultiplier = 0.5f;
        private const float CameraSensitivity = 0.1f;

        public void Construct(MobileHudController mobileHudController) => _mobileHudController = mobileHudController;

        public void ClearMobileHudController() => _mobileHudController = null;

        public Vector3 GetInputAxis() => _mobileHudController == null
            ? Vector3.zero
            : new Vector3(_mobileHudController.JoystickController.InputAxis.x, 0,
                _mobileHudController.JoystickController.InputAxis.y);

        public bool ShouldSwitchCameraMode() => _mobileHudController != null &&
                                                _mobileHudController.SwitchCameraHoldableButtonController.IsHold();

        public bool ShouldJump() => _mobileHudController != null && _mobileHudController.JumpHoldableButtonController.IsHold();

        public CameraInputData GetCameraInputData()
        {
            if (UnityEngine.Input.touchCount <= 0 || _mobileHudController == null) return new CameraInputData();

            var touches = Enumerable.Range(0, UnityEngine.Input.touchCount).Select(UnityEngine.Input.GetTouch).ToList();

            var movingTouchIndex =
                touches.FindIndex(touch => touch.phase == TouchPhase.Moved && IsTouchInLookArea(touch.position));

            if (movingTouchIndex == -1) return new CameraInputData();

            var movingTouch = touches[movingTouchIndex];

            return new CameraInputData(true, new Vector2(-movingTouch.deltaPosition.x,
                movingTouch.deltaPosition.y) * CameraSensitivity);
        }

        private bool IsTouchInLookArea(Vector2 touchPosition) =>
            touchPosition.x >= Screen.width * Mathf.Clamp01(_lookAreaScreenMultiplier);
    }
}