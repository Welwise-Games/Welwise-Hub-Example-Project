using TMPro;
using UnityEngine;
using WelwiseCharacterModule.Runtime.Client.Scripts.PlayerComponents;
using WelwiseChatModule.Runtime.Client.Scripts.UI;
using WelwiseClothesSharedModule.Runtime.Client.Scripts;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations;
using WelwiseSharedModule.Runtime.Client.Scripts;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts
{
    public class ClientPlayerSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public ToCameraLooker NicknameLooker { get; private set; }
        [field: SerializeField] public SkinColorChangerSerializableComponents SkinColorChangerSerializableComponents { get; private set; }
        [field: SerializeField] public ColorableClothesViewSerializableComponents ColorableClothesViewSerializableComponents { get; private set; }
        [field: SerializeField] public PlayerChatTextSerializableComponents ChatTextSerializableComponents { get; private set; }
        [field: SerializeField] public TextMeshProUGUI NicknameText { get; private set; }
        [field: SerializeField] public ClientPlayerCharacterSerializableComponents CharacterSerializableComponents { get; private set; }
    }
}