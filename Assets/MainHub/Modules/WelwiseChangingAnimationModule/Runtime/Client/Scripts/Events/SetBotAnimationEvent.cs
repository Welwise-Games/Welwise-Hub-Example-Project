using UnityEngine;
using WelwiseChangingAnimationModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.EventBusSystem;

namespace WelwiseChangingAnimationModule.Runtime.Client.Scripts.Events
{
    public struct SetBotAnimationEvent : IEvent
    {
        public readonly bool ShouldStartAnimation;
        public readonly AnimationType AnimationType;
        public readonly int BotObjectId;
        public readonly Vector3 RequiredBotTransformForward;
        public readonly Vector3 Position;
        public readonly int SenderId;

        public SetBotAnimationEvent(bool shouldStartAnimation, AnimationType animationType,
            Vector3 requiredBotTransformForward, int senderId, int botObjectId, Vector3 position)
        {
            ShouldStartAnimation = shouldStartAnimation;
            AnimationType = animationType;
            RequiredBotTransformForward = requiredBotTransformForward;
            SenderId = senderId;
            BotObjectId = botObjectId;
            Position = position;
        }
    }
}