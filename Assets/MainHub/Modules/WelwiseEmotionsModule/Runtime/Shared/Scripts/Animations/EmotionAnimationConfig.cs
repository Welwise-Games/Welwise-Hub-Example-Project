using System;
using UnityEngine;
using WelwiseItemInShopModule.Shared.Scripts;

namespace WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations
{
    [Serializable]
    public class EmotionAnimationConfig : IIndexableItemConfig
    {
        [field: SerializeField] public string Index { get; private set; }
        [field: SerializeField] [field: Range(0.1f, 100)] public float AnimationLength { get; private set; } 
        [field: SerializeField] public AnimatorOverrideController OverrideController { get; private set; }
    }
}