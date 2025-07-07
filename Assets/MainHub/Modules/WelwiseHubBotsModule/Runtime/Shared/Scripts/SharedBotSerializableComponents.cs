using FishNet.Component.Transforming;
using UnityEngine;
using UnityEngine.AI;
using WelwiseCharacterModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations;

namespace WelwiseHubBotsModule.Runtime.Shared.Scripts
{
    public class SharedBotSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public HeroAnimatorSerializableComponents HeroAnimatorSerializableComponents { get; private set; }
        [field: SerializeField] public NavMeshAgent NavMeshAgent { get; private set; }
        [field: SerializeField] public NetworkTransform NetworkTransform { get; private set; }
        [field: SerializeField] public PlayerEmotionsSerializableComponents EmotionsSerializableComponents { get; private set; }
    }
}