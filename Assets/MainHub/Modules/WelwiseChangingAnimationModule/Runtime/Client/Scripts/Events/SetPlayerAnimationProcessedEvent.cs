using UnityEngine;
using WelwiseChangingAnimationModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.EventBusSystem;

namespace WelwiseChangingAnimationModule.Runtime.Client.Scripts.Events
{
    public struct SetPlayerAnimationProcessedEvent : IEvent
    {
        public readonly bool ShouldStartAnimation;
        public readonly AnimationType AnimationType;
        public readonly Vector3 Position;
        public readonly Vector3 RequiredPlayerTransformForward;
        public readonly float CharacterControllerHeight;
        public readonly int SenderId;
        public readonly bool ForOwner;

        public SetPlayerAnimationProcessedEvent(AnimationType animationType, Vector3 position, int senderId,
            Vector3 requiredPlayerTransformForward, bool shouldStartAnimation, bool forOwner,
            float characterControllerHeight)
        {
            AnimationType = animationType;
            Position = position;
            SenderId = senderId;
            RequiredPlayerTransformForward = requiredPlayerTransformForward;
            ShouldStartAnimation = shouldStartAnimation;
            ForOwner = forOwner;
            CharacterControllerHeight = characterControllerHeight;
        }
    }
}