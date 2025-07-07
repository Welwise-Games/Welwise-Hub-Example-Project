using UnityEngine;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.HubSystem
{
    public class AutoRotate : MonoBehaviour
    {
        public enum Axis { X, Y, Z }
        [Header("Rotation Settings")]
        public Axis rotationAxis = Axis.Y;
        public float rotationSpeed = 90f; // �������� � �������

        private float currentAngle = 0f;

        void Update()
        {
            currentAngle += rotationSpeed * Time.deltaTime;

            Vector3 euler = transform.localEulerAngles;

            switch (rotationAxis)
            {
                case Axis.X:
                    euler.x = currentAngle;
                    break;
                case Axis.Y:
                    euler.y = currentAngle;
                    break;
                case Axis.Z:
                    euler.z = currentAngle;
                    break;
            }

            transform.localEulerAngles = euler;
        }
    }
}
