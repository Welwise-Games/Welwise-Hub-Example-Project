using UnityEngine;
using WelwiseItemInShopModule.Shared.Scripts;

namespace WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations
{
    [CreateAssetMenu(menuName = "WelwiseEmotionsModule/EmotionsAnimationsConfig")]
    public class EmotionsAnimationsConfig : ScriptableObject, IItemsConfig<EmotionAnimationConfig>
    {
        [field: Min(1)] [field: SerializeField] public int MaxSelectedItemsNumber { get; private set; } = 8;
        [field: SerializeField] public EmotionAnimationConfig[] Configs { get; private set; }
    }
}