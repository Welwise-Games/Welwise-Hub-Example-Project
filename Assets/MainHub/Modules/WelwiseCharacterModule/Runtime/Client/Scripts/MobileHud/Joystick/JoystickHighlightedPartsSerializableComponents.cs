using UnityEngine;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.MobileHud.Joystick
{
    public class JoystickHighlightedPartsSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public JoystickHighlightedPartSerializableComponents[] JoystickHighlightedPartSerializableComponents {get; private set; }
        [field: SerializeField] public PointerDragObserver PointerDragObserver { get; private set; }
    }
}