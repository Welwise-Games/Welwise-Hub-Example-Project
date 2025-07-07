using System.Collections.Generic;
using FishNet.Connection;
using WelwiseChatModule.Runtime.Shared.Scripts.Network;

namespace Modules.WelwiseChatModule.Runtime.Server.Scripts
{
    public interface IServerChatsDataProvider
    {
        void TryProcessingAndAddingChatMessageData(NetworkConnection networkConnection, ChatZone chatZone,
            ChatMessageData chatMessageData, out bool successfully, out string processedMessageContent);

        IReadOnlyDictionary<ChatZone, IReadOnlyCollection<ChatMessageData>> GetChatsMessagesData(
            NetworkConnection networkConnection);
    }
}