using System.Collections.Generic;
using UnityEngine;
using WelwiseItemInShopModule.Client.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts
{
    [CreateAssetMenu(menuName = "WeiwiseEmotionsModule/EmotionsViewConfig")]
    public class EmotionsViewConfig : ScriptableObject, IItemsViewConfig<EmotionViewConfig>
    {
        [field: SerializeField] public KeyCode[] OpenCircleKeycodes { get; private set; } = {KeyCode.Q, KeyCode.Mouse2};
        [field: SerializeField] public EmotionViewConfig[] Configs { get; private set; }
        [field: SerializeField] public ErrorTextConfig ErrorTextConfig { get; private set; }
        [field: SerializeField] [field: Range(1, 10)] public float MaxParticlesLifeTime { get; private set; } = 5;
    }
}