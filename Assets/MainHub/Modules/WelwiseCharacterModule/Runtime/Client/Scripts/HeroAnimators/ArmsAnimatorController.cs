using UnityEngine;
using WelwiseSharedModule.Runtime.Shared.Scripts;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.HeroAnimators
{
    public class ArmsAnimatorController
    {
        private readonly ArmsAnimatorSerializableComponents _armsAnimatorSerializableComponents;
        private readonly int _isRunningHash = Animator.StringToHash("isRunning");
        private readonly int _jumpHash = Animator.StringToHash("jump");

        public ArmsAnimatorController(ArmsAnimatorSerializableComponents armsAnimatorSerializableComponents) =>
            _armsAnimatorSerializableComponents = armsAnimatorSerializableComponents;

        public void TriggerJump() => _armsAnimatorSerializableComponents.Animator.SetTrigger(_jumpHash);

        public void SetIsRunning(bool isRunning) =>
            _armsAnimatorSerializableComponents.Animator.SetBool(_isRunningHash, isRunning);
    }
}