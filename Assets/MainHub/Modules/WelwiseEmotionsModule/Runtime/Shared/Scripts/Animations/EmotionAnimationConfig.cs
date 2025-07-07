using System;
using UnityEngine;

namespace WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations
{
    [Serializable]
    public class EmotionAnimationConfig
    {
        [field: SerializeField] public string EmotionIndex { get; private set; }
        [field: SerializeField] [field: Range(0.1f, 100)] public float AnimationLength { get; private set; } 
        [field: SerializeField] public AnimatorOverrideController OverrideController { get; private set; }
    }
}