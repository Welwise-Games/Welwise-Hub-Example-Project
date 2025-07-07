using UnityEngine;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.InputServices
{
    [CreateAssetMenu(fileName = "InputConfig", menuName = "WelwiseCharacterModule/InputConfig")]
    public class InputConfig : ScriptableObject
    {
        [field: SerializeField] public KeyCode SwitchCursorKeyCode { get; private set; } = KeyCode.Tab;
        [field: SerializeField] public KeyCode SwitchCursorCameraMode { get; private set; } = KeyCode.V;
    }
}