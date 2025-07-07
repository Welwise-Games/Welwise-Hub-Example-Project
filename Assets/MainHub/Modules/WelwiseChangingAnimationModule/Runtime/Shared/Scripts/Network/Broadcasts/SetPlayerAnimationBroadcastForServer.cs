using FishNet.Broadcast;
using UnityEngine;

namespace WelwiseChangingAnimationModule.Runtime.Shared.Scripts.Network.Broadcasts
{
    public struct SetPlayerAnimationBroadcastForServer : IBroadcast
    {
        public readonly int SenderId;

        public SetPlayerAnimationBroadcastForServer(int senderId)
        {
            SenderId = senderId;
        }
    }
}