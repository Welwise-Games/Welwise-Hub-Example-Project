using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseSharedModule.Runtime.Client.Scripts.Animator
{
    public class AnimatorStateReporter : StateMachineBehaviour
    {
        private List<IExitedAnimatorStateReader> _exitedStateReaders;
        private List<IAnimationStateReader> _stateReaders;
        
        private int _nextNormalizedTimeToInvokeOnEndState;
        private bool _didInvokeEndState;
        
        public override void OnStateEnter(UnityEngine.Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            FindReaders(animator);

            _nextNormalizedTimeToInvokeOnEndState = 1;
            _stateReaders.ForEach(reader => reader.OnStartState(stateInfo.shortNameHash));
            _stateReaders.ForEach(reader => reader.OnEnterState(stateInfo.shortNameHash));
        }

        public override void OnStateExit(UnityEngine.Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            _exitedStateReaders.ForEach(stateReader => stateReader?.OnExitState(stateInfo.shortNameHash));
            _didInvokeEndState = false;
        }

        public override void OnStateUpdate(UnityEngine.Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            _exitedStateReaders.OfType<IAnimationStateReader>().ForEach(reader => reader.OnUpdateState(stateInfo));
            
            if (stateInfo.normalizedTime < _nextNormalizedTimeToInvokeOnEndState) return;

            if (_didInvokeEndState)
            {
                _nextNormalizedTimeToInvokeOnEndState++;
                _stateReaders.ForEach(reader => reader.OnStartState(stateInfo.shortNameHash));
                _didInvokeEndState = false;
                return;
            }
            
            _exitedStateReaders.ForEach(reader => reader.OnEndState(stateInfo.shortNameHash));
            _didInvokeEndState = true;
        }

        private void FindReaders(UnityEngine.Animator animator)
        {
            if (_exitedStateReaders != null && _exitedStateReaders.All(reader => reader != null))
                return;

            _exitedStateReaders = animator.transform.GetComponents<IExitedAnimatorStateReader>().ToList();

            _exitedStateReaders = _exitedStateReaders.GroupBy(group => group).Select(group => group.First()).ToList();
            _stateReaders = _exitedStateReaders.OfType<IAnimationStateReader>().ToList();
        }
    }
}