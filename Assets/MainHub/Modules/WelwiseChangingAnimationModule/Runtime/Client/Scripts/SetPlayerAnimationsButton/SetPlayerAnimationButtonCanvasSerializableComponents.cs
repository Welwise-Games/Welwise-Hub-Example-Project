using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WelwiseSharedModule.Runtime.Client.Scripts;

namespace WelwiseChangingAnimationModule.Runtime.Client.Scripts.SetPlayerAnimationsButton
{
    public class SetPlayerAnimationButtonCanvasSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public ToCameraLooker ToCameraLooker { get; private set; }
        [field: SerializeField] public TextMeshProUGUI PressButtonKeyCodeText { get; private set; }
        [field: SerializeField] public Transform PressButtonKeyCodeTextParent { get; private set; }
        [field: SerializeField] public Button Button { get; private set; }
    }
}