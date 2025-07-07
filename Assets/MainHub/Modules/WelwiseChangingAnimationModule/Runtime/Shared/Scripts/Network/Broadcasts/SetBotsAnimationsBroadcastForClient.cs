using System.Collections.Generic;
using FishNet.Broadcast;

namespace WelwiseChangingAnimationModule.Runtime.Shared.Scripts.Network.Broadcasts
{
    public struct SetBotsAnimationsBroadcastForClient : IBroadcast
    {
        public readonly List<SetBotAnimationBroadcastForClient> Broadcasts;

        public SetBotsAnimationsBroadcastForClient(List<SetBotAnimationBroadcastForClient> broadcasts)
        {
            Broadcasts = broadcasts;
        }
    }
}