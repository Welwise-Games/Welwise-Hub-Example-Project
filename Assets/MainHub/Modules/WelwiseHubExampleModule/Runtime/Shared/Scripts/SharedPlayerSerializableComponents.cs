using UnityEngine;
using WelwiseChangingAnimationModule.Runtime.Shared.Scripts;
using WelwiseCharacterModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts
{
    public class SharedPlayerSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public PlayerEmotionsSerializableComponents EmotionsSerializableComponents { get; private set; }
        [field: SerializeField] public SharedPlayerCharacterSerializableComponents CharacterSerializableComponents { get; private set; }
        [field: SerializeField] public PlayerAnimatorSerializableComponents AnimatorSerializableComponents { get; private set; }
        [field: SerializeField] public MonoBehaviourObserver MonoBehaviourObserver { get; private set; }
    }
}