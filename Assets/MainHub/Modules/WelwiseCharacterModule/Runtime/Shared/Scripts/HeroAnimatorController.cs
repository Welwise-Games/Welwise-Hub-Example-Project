using FishNet.Component.Animating;
using UnityEngine;

namespace WelwiseCharacterModule.Runtime.Shared.Scripts
{
    public class HeroAnimatorController
    {
        public static readonly int RunHash = Animator.StringToHash("Run");
        
        private static readonly int _isRunningHash = Animator.StringToHash("isRunning");
        private static readonly int _isFallingHash = Animator.StringToHash("isFalling");
        private static readonly int _jumpHash = Animator.StringToHash("jump");
        private readonly Animator _animator;
        private readonly NetworkAnimator _networkAnimator;

        public HeroAnimatorController(HeroAnimatorSerializableComponents animatorSerializableComponents)
        {
            _animator = animatorSerializableComponents.Animator;
            _networkAnimator = animatorSerializableComponents.NetworkAnimator;
        }

        public HeroAnimatorController(Animator animator, NetworkAnimator networkAnimator)
        {
            _animator = animator;
            _networkAnimator = networkAnimator;
        }

        public void TriggerJump() => _networkAnimator.SetTrigger(_jumpHash);

        public void SetIsRunning(bool isRunning) => _animator.SetBool(_isRunningHash, isRunning);

        public void SetIsFalling(bool isFalling) =>
            _animator.SetBool(_isFallingHash, isFalling);
    }
}