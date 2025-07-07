using System.Collections.Generic;
using FishNet.Broadcast;

namespace WelwiseChangingAnimationModule.Runtime.Shared.Scripts.Network.Broadcasts
{
    public struct SetPlayersAnimationsBroadcastForClient : IBroadcast
    {
        public readonly List<SetPlayerAnimationBroadcastForClient> Broadcasts;

        public SetPlayersAnimationsBroadcastForClient(List<SetPlayerAnimationBroadcastForClient> broadcasts)
        {
            Broadcasts = broadcasts;
        }
    }
}