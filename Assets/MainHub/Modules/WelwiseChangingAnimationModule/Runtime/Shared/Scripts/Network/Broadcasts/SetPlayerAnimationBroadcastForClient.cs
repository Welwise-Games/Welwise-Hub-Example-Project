using FishNet.Broadcast;
using FishNet.Connection;

namespace WelwiseChangingAnimationModule.Runtime.Shared.Scripts.Network.Broadcasts
{
    public struct SetPlayerAnimationBroadcastForClient : IBroadcast
    {
        public readonly bool ShouldStartAnimation;
        public readonly int SetAnimationSenderId;
        public readonly int? PastBusySetAnimationSenderId;
        public readonly bool ForOwner;

        public SetPlayerAnimationBroadcastForClient(int setAnimationSenderId, int? pastBusySetAnimationSenderId,
            bool shouldStartAnimation, bool forOwner)
        {
            SetAnimationSenderId = setAnimationSenderId;
            PastBusySetAnimationSenderId = pastBusySetAnimationSenderId;
            ShouldStartAnimation = shouldStartAnimation;
            ForOwner = forOwner;
        }
    }
}