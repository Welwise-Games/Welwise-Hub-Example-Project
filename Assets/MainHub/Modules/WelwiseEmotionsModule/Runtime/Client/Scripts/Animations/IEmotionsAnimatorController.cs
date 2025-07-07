using System;
using UnityEngine;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts.Animations
{
    public interface IEmotionsAnimatorController
    {
        event Action<string, int> StartedEmotionAnimation;
        void SetAnimatorControllerAndTryStartingEmotionAnimation(AnimatorOverrideController animatorOverrideController,
            string emotionIndex, int emotionIndexInsideCircle = 0);
    }
}