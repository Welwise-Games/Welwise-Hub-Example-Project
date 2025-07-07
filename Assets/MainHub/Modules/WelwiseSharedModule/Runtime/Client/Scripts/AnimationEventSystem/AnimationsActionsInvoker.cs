using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WelwiseSharedModule.Runtime.Client.Scripts.Animator;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseSharedModule.Runtime.Client.Scripts.AnimationEventSystem
{
    public class AnimationsActionsInvoker
    {
        private readonly List<AnimationActionInfo> _animationActionsInfo;
        private readonly int _targetAnimationHash;

        public AnimationsActionsInvoker(AnimatorStateObserver animatorStateObserver,
            List<AnimationActionInfo> animationActionsInfo, int targetAnimationHash)
        {
            _animationActionsInfo = animationActionsInfo;
            _targetAnimationHash = targetAnimationHash;
            MakeAreNotInvokedAnimationActionsInfo(_targetAnimationHash);

            animatorStateObserver.ExitedState += MakeAreNotInvokedAnimationActionsInfo;
            animatorStateObserver.StartedState += MakeAreNotInvokedAnimationActionsInfo;
            animatorStateObserver.UpdatedState += TryInvokingAnimationsActions;
        }

        private void MakeAreNotInvokedAnimationActionsInfo(int hash)
        {
            if (_targetAnimationHash == hash)
                _animationActionsInfo.ForEach(info => info.IsInvoked = false);
        }

        private void TryInvokingAnimationsActions(AnimatorStateInfo animatorStateInfo)
        {
            if (animatorStateInfo.shortNameHash != _targetAnimationHash) return;

            var normalizedTime = animatorStateInfo.normalizedTime == 1 ? 1 : animatorStateInfo.normalizedTime % 1;

            var pastAnimationTime = animatorStateInfo.length * normalizedTime;
            
            _animationActionsInfo.Where(info => !info.IsInvoked && pastAnimationTime >= info.StartTimeFunc.Invoke())
                .ForEach(info =>
                {
                    info.IsInvoked = true;
                    info.Action?.Invoke();
                });
        }
    }
}