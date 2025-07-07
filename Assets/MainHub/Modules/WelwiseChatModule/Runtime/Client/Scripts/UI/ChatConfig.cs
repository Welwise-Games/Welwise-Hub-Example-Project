using UnityEngine;

namespace WelwiseChatModule.Runtime.Client.Scripts.UI
{
    [CreateAssetMenu(fileName = "ChatConfig", menuName = "ChatConfig")]
    public class ChatConfig : ScriptableObject
    {
        [field: Min(0.1f)] [field: SerializeField] public float TimeShowingPlayerNicknameBeforeHiding { get; private set; } = 3;
        [field: SerializeField] public KeyCode SelectInputFieldOrSendMessageKeyCode { get; private set; } = KeyCode.Return;
        [field: SerializeField] public KeyCode SetOpenStateChatWindowPopupKeyCode { get; private set; } = KeyCode.T;
    }
}