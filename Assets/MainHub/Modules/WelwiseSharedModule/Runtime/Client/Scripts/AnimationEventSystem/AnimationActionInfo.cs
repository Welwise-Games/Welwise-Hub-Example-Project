using System;
using System.Linq;

namespace WelwiseSharedModule.Runtime.Client.Scripts.AnimationEventSystem
{
    public class AnimationActionInfo
    {
        public bool IsInvoked { get; set; }
        public Func<float> StartTimeFunc { get; private set; }
        public Action Action { get; private set; }
        
        private const float DefaultAnimationClipFrameRate = 30;

        public AnimationActionInfo(Func<float> startTimeFunc, Action action, bool isInvoked = false)
        {
            IsInvoked = isInvoked;
            StartTimeFunc = startTimeFunc;
            Action = action;
        }
        
        public AnimationActionInfo(Func<int> startFrameFunc, UnityEngine.Animator animator, Action action, bool isInvoked = false)
        {
            IsInvoked = isInvoked;
            StartTimeFunc = () => startFrameFunc.Invoke() * (1f / animator.GetCurrentAnimatorClipInfo(0).FirstOrDefault().clip?.frameRate ??
                             DefaultAnimationClipFrameRate);
            Action = action;
        }
    }
}