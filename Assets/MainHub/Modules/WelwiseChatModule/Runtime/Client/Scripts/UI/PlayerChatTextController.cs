using UnityEngine;
using WelwiseSharedModule.Runtime.Shared.Scripts;

namespace WelwiseChatModule.Runtime.Client.Scripts.UI
{
    public class PlayerChatTextController
    {
        public bool IsTextRootParentActive => _timer.IsCounting();
        
        private readonly ChatConfig _chatConfig;
        private readonly PlayerChatTextSerializableComponents _playerChatTextSerializableComponents;
        private readonly Timer _timer;

        public PlayerChatTextController(ChatConfig chatConfig, PlayerChatTextSerializableComponents playerChatTextSerializableComponents, Camera mainCamera)
        {
            _chatConfig = chatConfig;
            _playerChatTextSerializableComponents = playerChatTextSerializableComponents;
            
            _timer = new Timer(playerChatTextSerializableComponents.destroyCancellationToken);
            
            _playerChatTextSerializableComponents.TextLooker.Construct(mainCamera);
            
            _timer.Started += time => playerChatTextSerializableComponents.TextRootParent.gameObject.SetActive(true);
            _timer.Ended += () => playerChatTextSerializableComponents.TextRootParent.gameObject.SetActive(false);
            playerChatTextSerializableComponents.TextRootParent.gameObject.SetActive(false);
        }

        public void StartOrContinueShowingText(string text)
        {
            _playerChatTextSerializableComponents.Text.text = text;
            _timer.TryStoppingCountingTime();
            _timer.TryStartingCountingTime(_chatConfig.TimeShowingPlayerNicknameBeforeHiding);
        }
    }
}