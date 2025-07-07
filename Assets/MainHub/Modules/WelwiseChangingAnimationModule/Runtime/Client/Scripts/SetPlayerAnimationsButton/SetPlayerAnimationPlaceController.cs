using FishNet.Object;
using UnityEngine;
using WelwiseChangingAnimationModule.Runtime.Client.Scripts.Events;
using WelwiseSharedModule.Runtime.Client.Scripts.Tools;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.EventBusSystem;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseChangingAnimationModule.Runtime.Client.Scripts.SetPlayerAnimationsButton
{
    public class SetPlayerAnimationPlaceController
    {
        public int Id { get; }
        public SetPlayerAnimationPlaceSerializableComponents SerializableComponents { get; }

        private bool _isBusy;

        public SetPlayerAnimationPlaceController(
            SetPlayerAnimationPlaceSerializableComponents serializableComponents,
            EventBus eventBus, SetPlayerAnimationsButtonsConfig config, Camera camera,
            int id)
        {
            SerializableComponents = serializableComponents;
            Id = id;
            serializableComponents.ColliderObserver.Entered += collider => SetActiveButton();
            serializableComponents.ColliderObserver.Exited += collider => SetActiveButton();

            serializableComponents.CanvasSerializableComponents.Button.onClick.AddListener(() =>
                FireSetOwnerAnimationAndPositionEvent(eventBus));

            serializableComponents.CanvasSerializableComponents.ToCameraLooker.Construct(camera);

            SubscribeToSetAnimationEventOnButtonPressed(config, eventBus);

            eventBus.Subscribe<StoppedPlayerAnimationProcessedEvent>(TrySettingIsBusyOnEntityStartedAnimationOnStoppedPlayerAnimation);
            eventBus.Subscribe<SetPlayerAnimationProcessedEvent>(TrySettingIsBusyOnEntityStartedAnimationForOnPlayerAnimation);
            eventBus.Subscribe<SetBotAnimationEvent>(TrySettingIsBusyOnEntityStartedAnimationOnSetBotAnimationEvent);

            serializableComponents.gameObject.GetOrAddComponent<DestroyObserver>().Destroyed
                += () =>
                {
                    eventBus.Unsubscribe<StoppedPlayerAnimationProcessedEvent>(TrySettingIsBusyOnEntityStartedAnimationOnStoppedPlayerAnimation);
                    eventBus.Unsubscribe<SetPlayerAnimationProcessedEvent>(TrySettingIsBusyOnEntityStartedAnimationForOnPlayerAnimation);
                    eventBus.Unsubscribe<SetBotAnimationEvent>(TrySettingIsBusyOnEntityStartedAnimationOnSetBotAnimationEvent);
                };
            
            void TrySettingIsBusyOnEntityStartedAnimationOnStoppedPlayerAnimation(StoppedPlayerAnimationProcessedEvent @event)
                => TrySettingIsBusyOnEntityStartedAnimation(@event.SenderId, false);

            void TrySettingIsBusyOnEntityStartedAnimationForOnPlayerAnimation(SetPlayerAnimationProcessedEvent @event)
                => TrySettingIsBusyOnEntityStartedAnimation(@event.SenderId, @event.ShouldStartAnimation);

            void TrySettingIsBusyOnEntityStartedAnimationOnSetBotAnimationEvent(SetBotAnimationEvent @event)
                => TrySettingIsBusyOnEntityStartedAnimation(@event.SenderId, @event.ShouldStartAnimation);


            serializableComponents.CanvasSerializableComponents.Button.gameObject.SetActive(false);

            serializableComponents.ColliderObserver.EnteredColliders.ForEach(collider =>
                SetActiveButton());

            SetActiveButton();

            serializableComponents.CanvasSerializableComponents.PressButtonKeyCodeTextParent.gameObject.SetActive(
                !DeviceDetectorTools.IsMobile());

            if (!DeviceDetectorTools.IsMobile())
                serializableComponents.CanvasSerializableComponents.PressButtonKeyCodeText.text =
                    config.SetAnimationKeyCode.ToString();
        }

        private void TrySettingIsBusyOnEntityStartedAnimation(int senderId, bool isBusy)
        {
            if (senderId != Id) return;

            _isBusy = isBusy;
            SetActiveButton();
        }

        void FireSetOwnerAnimationAndPositionEvent(EventBus eventBus) =>
            eventBus.Fire(
                new SetPlayerAnimationUnprocessedEvent(
                    SerializableComponents.PositionAndPlayerForwardDirectionProvider.position, Id));

        private void SubscribeToSetAnimationEventOnButtonPressed(
            SetPlayerAnimationsButtonsConfig config, EventBus eventBus)
        {
            if (!DeviceDetectorTools.IsMobile())
            {
                SerializableComponents.MonoBehaviourObserver.Updated += () =>
                {
                    if (_isBusy || SerializableComponents.ColliderObserver.EnteredColliders.Count == 0 ||
                        !Input.GetKeyDown(config.SetAnimationKeyCode)) return;

                    if (Input.GetKeyDown(config.SetAnimationKeyCode))
                        FireSetOwnerAnimationAndPositionEvent(eventBus);
                };
            }
        }
        
        
        private void SetActiveButton() =>
            SerializableComponents.CanvasSerializableComponents.Button.gameObject.SetActive(
                !_isBusy && SerializableComponents.ColliderObserver.EnteredColliders.Count > 0);
    }
}