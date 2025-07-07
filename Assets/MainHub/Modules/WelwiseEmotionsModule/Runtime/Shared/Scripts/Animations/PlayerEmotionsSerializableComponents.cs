using UnityEngine;

namespace WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations
{
    public class PlayerEmotionsSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public Animator Animator { get; private set; }
    }
}