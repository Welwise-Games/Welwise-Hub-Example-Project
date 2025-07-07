using System.Collections.Generic;
using UnityEngine;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts
{
    [CreateAssetMenu(fileName = "EmotionsModule/EmotionsViewConfig", menuName = "EmotionsModule/EmotionsViewConfig")]
    public class EmotionsViewConfig : ScriptableObject
    {
        [field: SerializeField] public List<KeyCode> OpenCircleKeycodes { get; private set; } = new List<KeyCode> {KeyCode.Q, KeyCode.Mouse2};
        [field: SerializeField] public EmotionViewConfig[] EmotionsConfigs { get; private set; }
        [field: SerializeField] public ErrorTextConfig ErrorTextConfig { get; private set; }
        [field: SerializeField] [field: Range(1, 10)] public float MaxParticlesLifeTime { get; private set; } = 5;
    }
}