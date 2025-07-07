using TMPro;
using UnityEngine;

namespace WebGLMobileKeyboardModule.Runtime.Scripts
{
    public class InputFieldMobileWebGLKeyboardInitializer : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;
        private void Start() => _inputField.InitializeInputFieldForMobileKeyboard();
    }
}