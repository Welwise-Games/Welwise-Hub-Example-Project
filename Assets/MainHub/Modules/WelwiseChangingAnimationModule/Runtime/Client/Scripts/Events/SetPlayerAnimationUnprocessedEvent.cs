using UnityEngine;
using WelwiseChangingAnimationModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.EventBusSystem;

namespace WelwiseChangingAnimationModule.Runtime.Client.Scripts.Events
{
    public struct SetPlayerAnimationUnprocessedEvent : IEvent
    {
        public readonly Vector3 Position;
        public readonly int SenderId;

        public SetPlayerAnimationUnprocessedEvent(Vector3 position, int senderId)
        {
            Position = position;
            SenderId = senderId;
        }
    }
}