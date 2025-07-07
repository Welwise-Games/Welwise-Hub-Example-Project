using System;
using UnityEngine;

namespace WelwiseSharedModule.Runtime.Shared.Scripts.Observers
{
    public class MonoBehaviourObserver : MonoBehaviour
    {
        public event Action Updated, LateUpdated;
        
        public void Update() => Updated?.Invoke();

        private void LateUpdate() => LateUpdated?.Invoke();
    }
}