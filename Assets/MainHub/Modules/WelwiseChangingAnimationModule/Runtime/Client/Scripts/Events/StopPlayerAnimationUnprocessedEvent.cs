using UnityEngine;
using WelwiseSharedModule.Runtime.Shared.Scripts.EventBusSystem;

namespace WelwiseChangingAnimationModule.Runtime.Client.Scripts.Events
{
    public struct StopPlayerAnimationUnprocessedEvent : IEvent
    {
        public readonly int SenderId;

        public StopPlayerAnimationUnprocessedEvent(int senderId)
        {
            SenderId = senderId;
        }
    }
}