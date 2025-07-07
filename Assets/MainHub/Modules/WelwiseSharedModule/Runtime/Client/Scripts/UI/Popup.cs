using System;
using UnityEngine;

namespace WelwiseSharedModule.Runtime.Client.Scripts.UI
{
    public class Popup : MonoBehaviour
    {
        public bool IsOpen { get; private set; }
        
        public event Action Opened, Closed;
        public event Action<bool> ChangedOpenState;

        [SerializeField] private bool _isOpenOnStart;


        private void Awake() => SetOpenState(_isOpenOnStart);

        public void TrySettingOpenState()
        {
            if (IsOpen)
                TryClosing();
            else
                TryOpening();
        }

        public void TrySettingOpenState(bool isOpen)
        {
            if (isOpen)
                TryOpening();
            else
                TryClosing();
        }

        public void TryOpening()
        {
            if (IsOpen)
                return;
            
            SetOpenState(true);
        }

        public void TryClosing()
        {
            if (!IsOpen)
                return;

            SetOpenState(false);
        }

        private void SetOpenState(bool isOpen)
        {
            IsOpen = isOpen;
            gameObject.SetActive(isOpen);

            if (isOpen)
                Opened?.Invoke();
            else
                Closed?.Invoke();
            
            ChangedOpenState?.Invoke(isOpen);
        }
    }
}