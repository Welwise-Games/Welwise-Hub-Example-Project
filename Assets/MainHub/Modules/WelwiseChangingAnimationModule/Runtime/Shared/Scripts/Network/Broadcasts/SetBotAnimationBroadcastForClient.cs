using FishNet.Broadcast;
using UnityEngine;

namespace WelwiseChangingAnimationModule.Runtime.Shared.Scripts.Network.Broadcasts
{
    public struct SetBotAnimationBroadcastForClient : IBroadcast
    {
        public readonly bool ShouldStartAnimation;
        public readonly int SetAnimationSenderId;
        public readonly int? PastBusySetAnimationSenderId;
        public readonly int BotObjectId;

        public SetBotAnimationBroadcastForClient(int setAnimationSenderId, int? pastBusySetAnimationSenderId,
            bool shouldStartAnimation, int botObjectId)
        {
            SetAnimationSenderId = setAnimationSenderId;
            PastBusySetAnimationSenderId = pastBusySetAnimationSenderId;
            ShouldStartAnimation = shouldStartAnimation;
            BotObjectId = botObjectId;
        }
    }
}