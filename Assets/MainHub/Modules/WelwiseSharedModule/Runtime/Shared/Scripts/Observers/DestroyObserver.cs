using System;
using UnityEngine;

namespace WelwiseSharedModule.Runtime.Shared.Scripts.Observers
{
    public class DestroyObserver : MonoBehaviour
    {
        public event Action Destroyed;
        private void OnDestroy() => Destroyed?.Invoke();
    }
}