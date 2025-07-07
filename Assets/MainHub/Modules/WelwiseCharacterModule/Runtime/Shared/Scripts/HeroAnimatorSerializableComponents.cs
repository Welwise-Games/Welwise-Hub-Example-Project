using FishNet.Component.Animating;
using UnityEngine;

namespace WelwiseCharacterModule.Runtime.Shared.Scripts
{
    public class HeroAnimatorSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public NetworkAnimator NetworkAnimator { get; private set; }
    }
}