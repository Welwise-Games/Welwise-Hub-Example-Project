using System.Collections.Generic;
using UnityEngine;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.OwnerPlayerMovement
{
    public class OwnerPlayerMovementSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public OwnerPlayerMovementConfig MovementConfig { get; private set; }
        [field: SerializeField] public MonoBehaviourObserver MonoBehaviourObserver { get; private set; }
        [field: SerializeField] public List<int> AnimationFramesWhenShouldPlayWalkingSounds { get; private set; }
    }
}