using UnityEngine;
using UnityEngine.EventSystems;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem
{
    public class PreviewRotatorController
    {
        private float _startPositionX;
        private readonly Transform _rotatingObject;
        private readonly float _sensitivity;

        public PreviewRotatorController(Transform rotatingObject, PointerUpDownObserver pointerUpDownObserver, PointerDragObserver pointerDragObserver, float sensitivity)
        {
            _rotatingObject = rotatingObject;
            _sensitivity = sensitivity;

            pointerDragObserver.Draged += Rotate;
            pointerUpDownObserver.PointerDowned += UpdateStartPositionX;
        }

        private void Rotate(PointerEventData eventData)
        {
            if (!_rotatingObject)
                return;

            var delta = eventData.position.x - _startPositionX;
            _startPositionX = eventData.position.x;

            _rotatingObject.Rotate(Vector3.up, -_sensitivity * delta);
        }

        private void UpdateStartPositionX(PointerEventData eventData)
        {
            _startPositionX = eventData.position.x;
        }
    }
}