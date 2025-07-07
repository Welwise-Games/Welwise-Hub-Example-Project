using FishNet.Broadcast;

namespace WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Network.Broadcasts
{
    public struct SettingNicknameBroadcastForServer : IBroadcast
    {
        public readonly string NewNickname;

        public SettingNicknameBroadcastForServer(string newNickname)
        {
            NewNickname = newNickname;
        }
    }
}