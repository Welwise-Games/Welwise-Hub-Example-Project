using UnityEngine;
using WelwiseCharacterModule.Runtime.Client.Scripts.MobileHud.Joystick;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.MobileHud
{
    public class MobileHudController
    {
        public JoystickController JoystickController { get; }
        public HoldableButtonController SwitchCameraHoldableButtonController { get; }
        public HoldableButtonController JumpHoldableButtonController { get; }

        public MobileHudController(MobileHudSerializableComponents mobileHudSerializableComponents)
        {
            mobileHudSerializableComponents.gameObject.SetActive(true);

            JoystickController = new JoystickController(mobileHudSerializableComponents.JoystickSerializableComponents);
            JumpHoldableButtonController = new HoldableButtonController(mobileHudSerializableComponents.JumpButtonPointerUpDownObserver);
            SwitchCameraHoldableButtonController = new HoldableButtonController(mobileHudSerializableComponents.SwitchCameraButtonPointerUpDownObserver);
        }
    }
}