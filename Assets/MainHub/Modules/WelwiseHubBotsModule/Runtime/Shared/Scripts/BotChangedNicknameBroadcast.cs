using FishNet.Broadcast;

namespace WelwiseHubBotsModule.Runtime.Shared.Scripts
{
    public struct BotChangedNicknameBroadcast : IBroadcast
    {
        public readonly int BotObjectId;
        public readonly string NewNickname;

        public BotChangedNicknameBroadcast(int botObjectId, string newNickname)
        {
            BotObjectId = botObjectId;
            NewNickname = newNickname;
        }
    }
}