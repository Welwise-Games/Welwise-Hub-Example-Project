using System;
using System.Collections.Generic;
using UnityEngine;

namespace WelwiseSharedModule.Runtime.Shared.Scripts.Observers
{
    public class ColliderObserver : MonoBehaviour
    {
        public IReadOnlyList<Collider> EnteredColliders
        {
            get
            {
                _enteredColliders.RemoveAll(collider => !collider);
                return _enteredColliders;
            }
        }

        public event Action<Collider> Entered, Exited;

        private readonly List<Collider> _enteredColliders = new List<Collider>();

        private void OnTriggerEnter(Collider other) => OnEnter(other);
        private void OnTriggerExit(Collider other) => OnExit(other);
        private void OnTriggerStay(Collider other) => OnEnter(other);

        private void OnCollisionEnter(Collision other) => OnEnter(other.collider);
        private void OnCollisionExit(Collision other) => OnExit(other.collider);
        private void OnCollisionStay(Collision other) => OnEnter(other.collider);

        private void OnEnter(Collider other)
        {
            if (_enteredColliders.Contains(other))
                return;

            _enteredColliders.Add(other);
            Entered?.Invoke(other);
        }

        private void OnExit(Collider other)
        {
            _enteredColliders.Remove(other);
            Exited?.Invoke(other);
        }
    }
}