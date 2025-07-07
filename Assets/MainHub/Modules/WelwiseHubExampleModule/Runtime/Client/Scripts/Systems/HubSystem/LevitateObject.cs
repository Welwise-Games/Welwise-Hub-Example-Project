using UnityEngine;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.HubSystem
{
    /// <summary>
    /// Makes the object float (levitate) up and down smoothly with customizable parameters.
    /// </summary>
    public class LevitateObject : MonoBehaviour
    {
        [Header("Levitation Settings")]
        public float amplitude = 0.5f;       // How high the object moves up/down
        public float frequency = 1f;         // How fast the object oscillates
        public bool useLocalPosition = false; // Use localPosition instead of world position

        [Header("Axis Control")]
        public Vector3 movementAxis = Vector3.up; // Direction of levitation (e.g., Vector3.up or any other axis)

        private Vector3 startPosition;

        void Start()
        {
            startPosition = useLocalPosition ? transform.localPosition : transform.position;
        }

        void Update()
        {
            float offset = Mathf.Sin(Time.time * frequency) * amplitude;
            Vector3 newPosition = startPosition + movementAxis.normalized * offset;

            if (useLocalPosition)
                transform.localPosition = newPosition;
            else
                transform.position = newPosition;
        }
    }
}
