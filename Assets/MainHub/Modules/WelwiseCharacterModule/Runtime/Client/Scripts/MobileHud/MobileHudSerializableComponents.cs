using UnityEngine;
using WelwiseCharacterModule.Runtime.Client.Scripts.MobileHud.Joystick;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.MobileHud
{
    public class MobileHudSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public JoystickSerializableComponents JoystickSerializableComponents { get; private set; }
        [field: SerializeField] public PointerUpDownObserver JumpButtonPointerUpDownObserver { get; private set; }
        [field: SerializeField] public PointerUpDownObserver SwitchCameraButtonPointerUpDownObserver { get; private set; }
    }
}