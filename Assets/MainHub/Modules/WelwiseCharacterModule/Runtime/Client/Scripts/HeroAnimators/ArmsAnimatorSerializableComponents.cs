using UnityEngine;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.HeroAnimators
{
    public class ArmsAnimatorSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public Animator Animator { get; private set; }
    }
}