using System;
using UnityEngine;

namespace WebGLMobileKeyboardModule.Runtime.Scripts
{
    public class MobileKeyboardEventsObserver : MonoBehaviour
    {
        public event Action<string> ChangedValue;
        public event Action Submitted;
        public event Action Destroyed;
        
        public void OnSubmit() => Submitted?.Invoke();

        public void OnChangeValue(string text) => ChangedValue?.Invoke(text);

        private void OnDestroy() => Destroyed?.Invoke();
    }
}