using TMPro;
using UnityEngine;
using WelwiseClothesSharedModule.Runtime.Client.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts;

namespace WelwiseHubBotsModule.Runtime.Client.Scripts
{
    public class ClientBotSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI NicknameText { get; private set; }
        [field: SerializeField] public ToCameraLooker ToCameraLooker { get; private set; }
        [field: SerializeField] public SkinColorChangerSerializableComponents SkinColorChangerSerializableComponents { get; private set; }
        [field: SerializeField] public ColorableClothesViewSerializableComponents ColorableClothesViewSerializableComponents { get; private set; }
        [field: SerializeField] public Transform AnimatorChildrenParent { get; private set; }
    }
}