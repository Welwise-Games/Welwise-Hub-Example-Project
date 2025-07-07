using System;
using UnityEngine;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations;
using WelwiseSharedModule.Runtime.Client.Scripts.Animator;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.HubSystem
{
    public class StandEmotionsAnimatorController
    {
        public event Action EndedAnimation;

        private static readonly int _canExitFromEmotionAnimation = Animator.StringToHash("canExitFromEmotionAnimation");

        public StandEmotionsAnimatorController(EmotionsAnimatorController emotionsAnimatorController,
            Animator animator, AnimatorStateObserver animatorStateObserver)
        {
            animatorStateObserver.EndedState += async (enteredAnimationHash) =>
            {
                emotionsAnimatorController.InvokeEndedEmotionAnimation(enteredAnimationHash);
                
                EndedAnimation?.Invoke();
            };

            emotionsAnimatorController.StartedEmotionAnimation +=
                (_, _) => animator.SetBool(_canExitFromEmotionAnimation, false);
        }
    }
}