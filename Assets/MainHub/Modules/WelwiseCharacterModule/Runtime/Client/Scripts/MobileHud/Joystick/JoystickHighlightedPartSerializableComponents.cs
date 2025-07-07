using UnityEngine;
using UnityEngine.UI;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.MobileHud.Joystick
{
    public class JoystickHighlightedPartSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public JoystickDirection JoystickDirection { get; private set; }
        [field: SerializeField] public Image Image { get; private set; }
    }
}