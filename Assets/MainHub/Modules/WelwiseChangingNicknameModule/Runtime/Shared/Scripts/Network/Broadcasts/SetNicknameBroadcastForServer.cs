using FishNet.Broadcast;

namespace WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Network.Broadcasts
{
    public struct SetNicknameBroadcastForServer : IBroadcast
    {
        public readonly string NewNickname;

        public SetNicknameBroadcastForServer(string newNickname)
        {
            NewNickname = newNickname;
        }
    }
}