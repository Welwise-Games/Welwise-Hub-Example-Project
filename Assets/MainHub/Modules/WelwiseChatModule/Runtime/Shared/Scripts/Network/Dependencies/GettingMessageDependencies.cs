using FishNet.Broadcast;
using FishNet.Connection;

namespace WelwiseChatModule.Runtime.Shared.Scripts.Network.Dependencies
{
    public struct GettingMessageDependencies : IBroadcast
    {
        public readonly ChatMessageData MessageData;
        public readonly ChatZone ChatZone;
        public readonly NetworkConnection SenderNetworkConnection;

        public GettingMessageDependencies(ChatZone chatZone, ChatMessageData messageData, NetworkConnection senderNetworkConnection)
        {
            ChatZone = chatZone;
            MessageData = messageData;
            SenderNetworkConnection = senderNetworkConnection;
        }
    }
}