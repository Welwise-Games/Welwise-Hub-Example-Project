using UnityEngine;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.InputServices
{
    public class DekstopInputService : IDesktopInputService
    {
        private readonly InputConfig _inputConfig;

        private const string HorizontalAxis = "Horizontal";
        private const string VerticalAxis = "Vertical";

        public DekstopInputService(InputConfig inputConfig) => _inputConfig = inputConfig;

        public Vector3 GetInputAxis() =>
            new Vector3(UnityEngine.Input.GetAxisRaw(HorizontalAxis), 0,
                UnityEngine.Input.GetAxisRaw(VerticalAxis));

        public bool ShouldJump() => UnityEngine.Input.GetKeyDown(KeyCode.Space);

        public CameraInputData GetCameraInputData() =>
            new CameraInputData(Input.GetMouseButton(1), new Vector2(UnityEngine.Input.GetAxis("Mouse X"),
                -UnityEngine.Input.GetAxis("Mouse Y")));

        public bool ShouldSwitchCursor() => UnityEngine.Input.GetKeyDown(_inputConfig.SwitchCursorKeyCode);

        public bool ShouldSwitchCameraMode() => UnityEngine.Input.GetKeyDown(_inputConfig.SwitchCursorCameraMode);
    }
}