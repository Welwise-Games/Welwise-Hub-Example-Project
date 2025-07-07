using UnityEngine;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Logic
{
    public class PlanetRotator : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private Vector3 rotationAxis = Vector3.up;

        void Update()
        {
            transform.RotateAround(Vector3.zero, rotationAxis, rotationSpeed * Time.deltaTime);
        }
    }
}