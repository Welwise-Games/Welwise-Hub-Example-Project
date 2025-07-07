using UnityEngine;

namespace WelwiseSharedModule.Runtime.Client.Scripts.Animator
{
    public interface IAnimationStateReader : IExitedAnimatorStateReader
    {
        void OnStartState(int stateHash);
        void OnEnterState(int stateHash);
        void OnUpdateState(AnimatorStateInfo animatorStateInfo);
    }
}