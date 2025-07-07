using WelwiseChatModule.Runtime.Client.Scripts.UI;

namespace WelwiseChatModule.Runtime.Client.Scripts.Network
{
    public class ChatEntryPointData
    {
        public readonly ChatFactory ChatFactory;
        public readonly ChatsDataProviderService ChatsDataProviderService;

        public ChatEntryPointData(ChatFactory chatFactory, ChatsDataProviderService chatsDataProviderService)
        {
            ChatFactory = chatFactory;
            ChatsDataProviderService = chatsDataProviderService;
        }
    }
}