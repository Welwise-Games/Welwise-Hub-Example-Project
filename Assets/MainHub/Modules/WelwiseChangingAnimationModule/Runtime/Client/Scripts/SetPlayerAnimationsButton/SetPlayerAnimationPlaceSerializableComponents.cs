using UnityEngine;
using WelwiseChangingAnimationModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;

namespace WelwiseChangingAnimationModule.Runtime.Client.Scripts.SetPlayerAnimationsButton
{
    public class SetPlayerAnimationPlaceSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public ColliderObserver ColliderObserver { get; private set; }
        [field: SerializeField] public MonoBehaviourObserver MonoBehaviourObserver { get; private set; }
        [field: SerializeField] public AnimationType AnimationType { get; private set; }
        [field: SerializeField] [field: Range(0.1f, 10f)] public float CharacterControllerHeight { get; private set; } = 0.7f;
        [field: SerializeField] public Transform PositionAndPlayerForwardDirectionProvider { get; private set; }
        [field: SerializeField] public SetPlayerAnimationButtonCanvasSerializableComponents CanvasSerializableComponents {get; private set; }
    }
}