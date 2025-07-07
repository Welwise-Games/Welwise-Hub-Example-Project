using System;
using System.Collections;
using UnityEngine;

namespace WelwiseChangingAnimationModule.Runtime.Shared.Scripts
{
    public class DeferredActionInvoker
    {
        public bool DidCallInvokeActionInThisFrame { get; private set; }
        public bool CanCallAction { get; set; } = true;
        public event Action Action;
        private readonly MonoBehaviour _coroutineRunner;

        public DeferredActionInvoker(MonoBehaviour coroutineRunner) => _coroutineRunner = coroutineRunner;

        public void WaitEndFrameAndInvokeAction()
        {
            if (_coroutineRunner)
                _coroutineRunner.StartCoroutine(WaitEndFrameAndInvokeActionCoroutine());
        }

        private IEnumerator WaitEndFrameAndInvokeActionCoroutine()
        {
            if (DidCallInvokeActionInThisFrame || !CanCallAction)
                yield break;

            DidCallInvokeActionInThisFrame = true;
            yield return new WaitForEndOfFrame();

            if (CanCallAction)
                Action?.Invoke();

            DidCallInvokeActionInThisFrame = false;
        }
    }
}