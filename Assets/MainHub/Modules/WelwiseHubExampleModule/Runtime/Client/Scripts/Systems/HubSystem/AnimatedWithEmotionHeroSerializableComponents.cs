using UnityEngine;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.HubSystem
{
    public class AnimatedWithEmotionHeroSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public PlayerEmotionsSerializableComponents PlayerEmotionsSerializableComponents { get; private set; }
        [field: SerializeField] public string EmotionIndex { get; private set; }
    }
}