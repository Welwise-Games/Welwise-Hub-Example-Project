using FishNet.Component.Animating;
using UnityEngine;
using WelwiseChangingAnimationModule.Runtime.Client.Scripts.Events;
using WelwiseChangingAnimationModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts.Animator;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.EventBusSystem;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;

namespace WelwiseChangingAnimationModule.Runtime.Client.Scripts
{
    public class PlayerAnimatorController
    {
        private int _lastChangedAnimationHash;
        private int _lastDoesPlayAnimationVariableHash;
        private int _lastSetAnimationEventSenderHashCode;
        private bool _stoppedLastPlayedAnimation;

        private readonly PlayerAnimatorSerializableComponents _playerAnimatorSerializableComponents;
        private readonly CharacterController _characterController;

        public PlayerAnimatorController(EventBus eventBus,
            Transform playerTransform, PlayerAnimatorSerializableComponents playerAnimatorSerializableComponents,
            AnimatorStateObserver animatorStateObserver, CharacterController characterController)
        {
            _playerAnimatorSerializableComponents = playerAnimatorSerializableComponents;
            _characterController = characterController;

            eventBus.Subscribe<SetPlayerAnimationProcessedEvent>(TrySettingAnimationLocal);

            playerTransform.gameObject.GetOrAddComponent<DestroyObserver>().Destroyed
                += () => eventBus.Unsubscribe<SetPlayerAnimationProcessedEvent>(TrySettingAnimationLocal);

            void TrySettingAnimationLocal(SetPlayerAnimationProcessedEvent @event)
                => TrySettingAnimation(playerTransform, @event);

            animatorStateObserver.ExitedState += hash =>
                SendChangedAnimationOnEndIfAnimationIsChanged(eventBus, hash);
        }

        private void SendChangedAnimationOnEndIfAnimationIsChanged(EventBus eventBus, int hash)
        {
            if (_stoppedLastPlayedAnimation || hash != _lastChangedAnimationHash) return;

            _stoppedLastPlayedAnimation = true;

            eventBus.Fire(new StopPlayerAnimationUnprocessedEvent(
                _lastSetAnimationEventSenderHashCode));

            _playerAnimatorSerializableComponents.Animator.SetBool(_lastDoesPlayAnimationVariableHash, false);

            if (_characterController)
                _characterController.height =
                    _playerAnimatorSerializableComponents.CharacterControllerHeightOnDoesntSit;
        }

        private void TrySettingAnimation(Transform playerTransform,
            SetPlayerAnimationProcessedEvent processedEvent)
        {
            if (!processedEvent.ForOwner ||
                !AnimationHashes.HashesDataByAnimationType.TryGetValue(processedEvent.AnimationType, out var data) ||
                !processedEvent.ShouldStartAnimation && _stoppedLastPlayedAnimation)
                return;

            var shouldStartAnimation = processedEvent.ShouldStartAnimation;

            _stoppedLastPlayedAnimation = !shouldStartAnimation;

            _playerAnimatorSerializableComponents.Animator.SetBool(data.DoesPlayHash, shouldStartAnimation);

            if (processedEvent.AnimationType == AnimationType.Sit)
            {
                _characterController.height = shouldStartAnimation
                    ? processedEvent.CharacterControllerHeight
                    : _playerAnimatorSerializableComponents.CharacterControllerHeightOnDoesntSit;
            }

            if (!shouldStartAnimation) return;

            _lastSetAnimationEventSenderHashCode = processedEvent.SenderId;
            _lastDoesPlayAnimationVariableHash = data.DoesPlayHash;

            _characterController.enabled = false;

            _lastChangedAnimationHash = data.StateHash;
            
            playerTransform.position = processedEvent.Position;
            
            playerTransform.forward = processedEvent.RequiredPlayerTransformForward;

            _characterController.enabled = true;
        }
    }
}