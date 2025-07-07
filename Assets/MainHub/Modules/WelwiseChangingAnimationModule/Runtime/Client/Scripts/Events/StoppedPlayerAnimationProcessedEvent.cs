using WelwiseSharedModule.Runtime.Shared.Scripts.EventBusSystem;

namespace WelwiseChangingAnimationModule.Runtime.Client.Scripts.Events
{
    public struct StoppedPlayerAnimationProcessedEvent : IEvent
    {
        public readonly int SenderId;

        public StoppedPlayerAnimationProcessedEvent(int senderId)
        {
            SenderId = senderId;
        }
    }
}