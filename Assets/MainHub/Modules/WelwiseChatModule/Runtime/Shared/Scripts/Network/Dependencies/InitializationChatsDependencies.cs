using System.Collections.Generic;
using FishNet.Broadcast;

namespace WelwiseChatModule.Runtime.Shared.Scripts.Network.Dependencies
{
    public struct InitializationChatsDependencies : IBroadcast
    {
        public readonly Dictionary<ChatZone, List<ChatMessageData>> Chats;

        public InitializationChatsDependencies(Dictionary<ChatZone, List<ChatMessageData>> chats)
        {
            Chats = chats;
        }
    }
}