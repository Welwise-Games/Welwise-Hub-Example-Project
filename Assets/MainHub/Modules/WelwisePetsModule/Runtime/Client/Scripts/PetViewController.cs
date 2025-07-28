using UnityEngine;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;

namespace WelwisePetsModule.Runtime.Client.Scripts
{
    public class PetViewController
    {
        public readonly PetViewConfig ViewConfig;
        public readonly Transform ViewTransform;
        
        private readonly Transform _targetTransform;
        private readonly Vector3 _positionOffsetForTargetTransform;

        public PetViewController(Transform viewTransform, Transform targetTransform,
            Vector3 positionOffsetForTargetTransform, PetViewConfig viewConfig)
        {
            ViewTransform = viewTransform;
            _positionOffsetForTargetTransform = positionOffsetForTargetTransform;
            ViewConfig = viewConfig;
            _targetTransform = targetTransform;

            ViewTransform.gameObject.GetOrAddComponent<MonoBehaviourObserver>().Updated +=
                SetPositionAndRotation;
        }

        private void SetPositionAndRotation()
        {
            ViewTransform.position = Vector3.Lerp(ViewTransform.position,
                _targetTransform.position + _targetTransform.TransformDirection(_positionOffsetForTargetTransform), ViewConfig.Speed * Time.deltaTime);

            ViewTransform.forward = _targetTransform.forward;
        }
    }
}