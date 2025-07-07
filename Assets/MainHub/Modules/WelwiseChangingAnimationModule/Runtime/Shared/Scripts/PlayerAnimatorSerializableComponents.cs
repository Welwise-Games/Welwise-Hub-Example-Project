using UnityEngine;

namespace WelwiseChangingAnimationModule.Runtime.Shared.Scripts
{
    public class PlayerAnimatorSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] [field: Range(0.1f, 10f)] public float CharacterControllerHeightOnDoesntSit { get; private set; } = 2f;
    }
}