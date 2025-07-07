using System;
using UnityEngine;

namespace WelwiseSharedModule.Runtime.Client.Scripts.Animator
{
    public class AnimatorStateObserver : MonoBehaviour, IAnimationStateReader
    {
        public event Action<int> EnteredState, ExitedState, StartedState, EndedState;
        public event Action<AnimatorStateInfo> UpdatedState;
        public void OnEndState(int stateHash) => EndedState?.Invoke(stateHash);
        public void OnExitState(int stateHash) => ExitedState?.Invoke(stateHash);
        public void OnStartState(int stateHash) => StartedState?.Invoke(stateHash);
        public void OnEnterState(int stateHash) => EnteredState?.Invoke(stateHash);
        public void OnUpdateState(AnimatorStateInfo animatorStateInfo) => UpdatedState?.Invoke(animatorStateInfo);
    }
}