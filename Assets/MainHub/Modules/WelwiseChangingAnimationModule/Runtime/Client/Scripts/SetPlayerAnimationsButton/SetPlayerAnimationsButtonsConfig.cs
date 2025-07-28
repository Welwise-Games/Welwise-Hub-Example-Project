using UnityEngine;

namespace WelwiseChangingAnimationModule.Runtime.Client.Scripts.SetPlayerAnimationsButton
{
    [CreateAssetMenu(menuName = "WelwiseChangingAnimationModule/SetPlayerAnimationsButtonsConfig")]
    public class SetPlayerAnimationsButtonsConfig : ScriptableObject
    {
        [field: SerializeField] public KeyCode SetAnimationKeyCode { get; private set; }
    }
}