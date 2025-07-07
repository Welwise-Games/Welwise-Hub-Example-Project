using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using FishNet;
using FishNet.Component.Animating;
using UnityEngine;
using WelwiseSharedModule.Runtime.Client.Scripts.AnimationEventSystem;
using WelwiseSharedModule.Runtime.Client.Scripts.Animator;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts.Animations
{
    public class EmotionsAnimatorController : IEmotionsAnimatorController
    {
        public bool IsPlayingEmotion { get; private set; }

        public event Action<string, int> StartedEmotionAnimation;
        public event Action EndedEmotionAnimation;

        public readonly AnimatorStateObserver AnimatorStateObserver;
        public readonly ParticleEventController ParticleEventController;

        private int _targetEmotionIndexInsideCircle;
        private string _targetEmotionIndex;
        private EmotionViewConfig _targetEmotionViewConfig;

        private readonly Animator _animator;
        private readonly NetworkAnimator _networkAnimator;

        private readonly EmotionsViewConfig _emotionsViewConfig;

        private static readonly int _playEmotionHash = Animator.StringToHash("playEmotion");
        private static readonly int _playingEmotionHash = Animator.StringToHash("PlayingEmotion");


        public EmotionsAnimatorController(Animator animator, AnimatorStateObserver animatorStateObserver,
            ParticleEventController particleEventController, EmotionsViewConfig emotionsViewConfig,
            NetworkAnimator networkAnimator = null)
        {
            _animator = animator;
            AnimatorStateObserver = animatorStateObserver;
            ParticleEventController = particleEventController;
            _emotionsViewConfig = emotionsViewConfig;
            _networkAnimator = networkAnimator;

            animatorStateObserver.ExitedState += InvokeEndedEmotionAnimation;

            EndedEmotionAnimation += particleEventController.StopAllParticles;

            EndedEmotionAnimation += () => IsPlayingEmotion = false;
            StartedEmotionAnimation += (_, _) => IsPlayingEmotion = true;

            new AnimationsActionsInvoker(animatorStateObserver, new List<AnimationActionInfo>
            {
                new(() => _targetEmotionViewConfig?.FrameWhenPlayParticles ?? 0, animator,
                    particleEventController.PlayAllParticles)
            }, _playingEmotionHash);
        }

        public void InvokeEndedEmotionAnimation(int hash)
        {
            if (hash != _playingEmotionHash)
                return;

            EndedEmotionAnimation?.Invoke();
        }

        public void SetAnimatorControllerAndTryStartingEmotionAnimation(
            AnimatorOverrideController animatorOverrideController,
            string emotionIndex, int emotionIndexInsideCircle = -1)
        {
            var viewConfig =
                _emotionsViewConfig.EmotionsConfigs.FirstOrDefault(config => config?.EmotionIndex == emotionIndex);

            _targetEmotionViewConfig = viewConfig;

            if (_networkAnimator)
                _networkAnimator.SetController(animatorOverrideController);
            else
                _animator.runtimeAnimatorController = animatorOverrideController;

            if (_networkAnimator)
                _networkAnimator.SetTrigger(_playEmotionHash);
            else
                _animator.SetTrigger(_playEmotionHash);

            StartedEmotionAnimation?.Invoke(emotionIndex, emotionIndexInsideCircle);
        }
    }
}