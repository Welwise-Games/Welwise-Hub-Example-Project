using FishNet.Component.Animating;
using UnityEngine;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;

namespace WelwiseCharacterModule.Runtime.Shared.Scripts
{
    public class SharedPlayerCharacterSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public CharacterController CharacterController { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public NetworkAnimator NetworkAnimator { get; private set; }
        [field: SerializeField] public MonoBehaviourObserver MonoBehaviourObserver { get; private set; }
    }
}