using FishNet.Broadcast;

namespace WelwiseChatModule.Runtime.Shared.Scripts.Network.Dependencies
{
    public struct SendingMessageDependencies : IBroadcast
    {
        public readonly string Message;
        public readonly ChatZone ChatZone;

        public SendingMessageDependencies(string message, ChatZone chatZone)
        {
            Message = message;
            ChatZone = chatZone;
        }
    }
}