using UnityEngine;
using WelwiseCharacterModule.Runtime.Client.Scripts.HeroAnimators;
using WelwiseCharacterModule.Runtime.Client.Scripts.OwnerPlayerMovement;
using WelwiseCharacterModule.Runtime.Client.Scripts.PlayerCamera;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.PlayerComponents
{
    [RequireComponent(typeof(MonoBehaviourObserver))]
    public class OwnerPlayerCharacterSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public AudioSource WalkingAudioSource { get; private set; }
        [field: SerializeField] public AudioSource JumpAudioSource { get; private set; }
        [field: SerializeField] public CameraControllerSerializableComponents CameraControllerSerializableComponents { get; private set; }
        [field: SerializeField] public ArmsAnimatorSerializableComponents ArmsAnimatorSerializableComponents { get; private set; }
        [field: SerializeField] public OwnerPlayerMovementSerializableComponents MovementSerializableComponents { get; private set; }
        [field: SerializeField] public ClientPlayerCharacterSerializableComponents ClientCharacterSerializableComponents { get; private set; }
    }
}