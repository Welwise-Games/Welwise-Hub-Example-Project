using UnityEngine;

namespace WelwiseSharedModule.Runtime.Client.Scripts
{
    public class ToCameraLooker : MonoBehaviour
    {
        private Camera _mainCamera;

        public void Construct(Camera mainCamera) => _mainCamera = mainCamera;

        private void Update()
        {
            if (!_mainCamera)
            {
                //Debug.LogWarning("Main camera is not found!");
                return;
            }

            LookAtCamera();
        }

        private void LookAtCamera()
        {
            Vector3 directionToCamera = _mainCamera.transform.position - transform.position;

            directionToCamera.y = 0;

            if (directionToCamera != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(-directionToCamera);
            }
        }
    }
}