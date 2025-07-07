using UnityEngine;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.MobileHud.Joystick
{
    [RequireComponent(typeof(CanvasGroup))]
    public class JoystickSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public JoystickHighlightedPartsSerializableComponents HighlightedPartsSerializableComponents { get; private set; }
        [field: SerializeField] public CanvasGroup CanvasGroup { get; private set; }
        [field: SerializeField] public RectTransform JoystickBackground { get; private set; }
        [field: SerializeField] public RectTransform JoystickHandle { get; private set; }
        [field: SerializeField] public JoystickConfig JoystickConfig { get; private set; }
        [field: SerializeField] public MonoBehaviourObserver MonoBehaviourObserver { get; private set; }
        [field: SerializeField] public PointerDragObserver PointerDragObserver { get; private set; }
        [field: SerializeField] public PointerUpDownObserver PointerUpDownObserver { get; private set; }
    }
}