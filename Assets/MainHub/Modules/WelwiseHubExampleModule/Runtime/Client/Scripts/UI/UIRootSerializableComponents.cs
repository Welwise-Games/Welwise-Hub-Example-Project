using TMPro;
using UnityEngine;
using WelwiseCharacterModule.Runtime.Client.Scripts.MobileHud;
using WelwiseCharacterModule.Runtime.Client.Scripts.MobileHud.Joystick;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.UI
{
    public class UIRootSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public MobileHudSerializableComponents MobileHudSerializableComponents { get; private set; }
        [field: SerializeField] public ErrorTextConfig ErrorTextConfig { get; private set; }
        [field: SerializeField] public TextMeshProUGUI ErrorText { get; private set; }
    }
}