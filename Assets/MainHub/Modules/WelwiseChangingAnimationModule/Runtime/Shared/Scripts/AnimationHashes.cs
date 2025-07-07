using System.Collections.Generic;
using UnityEngine;

namespace WelwiseChangingAnimationModule.Runtime.Shared.Scripts
{
    public static class AnimationHashes
    {
        public static readonly Dictionary<AnimationType, AnimationHashesData> HashesDataByAnimationType
            = new Dictionary<AnimationType, AnimationHashesData>
            {
                {
                    AnimationType.Sit,
                    new AnimationHashesData(Animator.StringToHash("isSitting"), Animator.StringToHash("Sitting"))
                }
            };
    }
}