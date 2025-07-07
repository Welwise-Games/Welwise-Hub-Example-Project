using FishNet.Broadcast;
using FishNet.Connection;

namespace WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Network.Broadcasts
{
    public struct SettingNicknameBroadcastForClient : IBroadcast
    {
        public readonly string NewNickname;
        public readonly NetworkConnection NicknameOwnerNetworkConnection;

        public SettingNicknameBroadcastForClient(string newNickname, NetworkConnection nicknameOwnerNetworkConnection)
        {
            NewNickname = newNickname;
            NicknameOwnerNetworkConnection = nicknameOwnerNetworkConnection;
        }
    }
}