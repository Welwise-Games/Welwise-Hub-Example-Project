using FishNet.Object;
using UnityEngine;
using WelwiseChangingAnimationModule.Runtime.Client.Scripts.Events;
using WelwiseChangingAnimationModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.EventBusSystem;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;

namespace WelwiseChangingAnimationModule.Runtime.Client.Scripts
{
    public class BotAnimatorController
    {
        private readonly Animator _animator;

        private static readonly int _shouldWaitForPlayingEmotionEnd = Animator.StringToHash("shouldWaitForPlayingEmotionEnd");

        public BotAnimatorController(Animator animator,
            EventBus eventBus)
        {
            _animator = animator;
            eventBus.Subscribe<SetBotAnimationEvent>(TrySettingForwardLocal);

            animator.gameObject.GetOrAddComponent<DestroyObserver>().Destroyed += ()
                => eventBus.Unsubscribe<SetBotAnimationEvent>(TrySettingForwardLocal);

            void TrySettingForwardLocal(SetBotAnimationEvent @event) =>
                TrySettingForward(animator.transform, @event);
            
            animator.SetBool(_shouldWaitForPlayingEmotionEnd, true);
            
        }

        private void TrySettingForward(Transform botTransform, SetBotAnimationEvent @event)
        {
            if (!AnimationHashes.HashesDataByAnimationType.TryGetValue(@event.AnimationType, out var data) ||
                _animator.GetComponent<NetworkObject>().ObjectId != @event.BotObjectId)
                return;
            
            _animator.SetBool(data.DoesPlayHash, @event.ShouldStartAnimation);

            if (!@event.ShouldStartAnimation) return;
            botTransform.forward = @event.RequiredBotTransformForward;
            botTransform.position = @event.Position;
        }
    }
}