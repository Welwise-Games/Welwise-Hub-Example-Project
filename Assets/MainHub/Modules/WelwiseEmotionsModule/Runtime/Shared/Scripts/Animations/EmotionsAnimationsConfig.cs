using UnityEngine;

namespace WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations
{
    [CreateAssetMenu(fileName = "EmotionsModule/EmotionsAnimationsConfig",
        menuName = "EmotionsModule/EmotionsAnimationsConfig")]
    public class EmotionsAnimationsConfig : ScriptableObject
    {
        [field: Min(1)] [field: SerializeField] public int MaxSelectedAnimationsNumber { get; private set; } = 8;
        [field: SerializeField] public EmotionAnimationConfig[] EmotionsAnimationConfigs { get; private set; }
    }
}