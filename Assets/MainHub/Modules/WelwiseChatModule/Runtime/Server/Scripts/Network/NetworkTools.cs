using System.Linq;
using FishNet.Connection;
using FishNet.Managing.Server;
using WelwiseChatModule.Runtime.Shared.Scripts.Network;
using WelwiseChatModule.Runtime.Shared.Scripts.Network.Dependencies;

namespace Modules.WelwiseChatModule.Runtime.Server.Scripts.Network
{
    public static class NetworkTools
    {
        public static void SendInitializationChatsDataForClient(this IServerChatsDataProvider serverChatsDataProvider, NetworkConnection networkConnection, ServerManager serverManager)
        {
            var chatsMessagesData = serverChatsDataProvider.GetChatsMessagesData(networkConnection)?.ToDictionary(
                pair => pair.Key,
                pair => pair.Value.Select(data =>
                    new ChatMessageData(data.Content.GetProcessedMessageContentForServer(), data.AuthorNickname)).ToList());

            if (chatsMessagesData is { Count: > 0 })
                serverManager.Broadcast(networkConnection, new InitializationChatsDependencies(
                    chatsMessagesData));
        }
    }
}