using FishNet.Broadcast;
using FishNet.Connection;

namespace WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Network.Broadcasts
{
    public struct SetNicknameBroadcastForClient : IBroadcast
    {
        public readonly string NewNickname;
        public readonly NetworkConnection NicknameOwnerNetworkConnection;

        public SetNicknameBroadcastForClient(string newNickname, NetworkConnection nicknameOwnerNetworkConnection)
        {
            NewNickname = newNickname;
            NicknameOwnerNetworkConnection = nicknameOwnerNetworkConnection;
        }
    }
}