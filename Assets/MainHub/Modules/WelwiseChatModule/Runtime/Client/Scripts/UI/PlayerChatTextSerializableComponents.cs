using TMPro;
using UnityEngine;
using WelwiseSharedModule.Runtime.Client.Scripts;

namespace WelwiseChatModule.Runtime.Client.Scripts.UI
{
    public class PlayerChatTextSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI Text { get; private set; }
        [field: SerializeField] public Transform TextRootParent { get; private set; }
        [field: SerializeField] public ToCameraLooker TextLooker { get; private set; }
    }
}