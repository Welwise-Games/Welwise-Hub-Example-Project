using UnityEngine;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts
{
    public class ParticlesParentSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public GameObject[] OptionalParticles { get; private set; }
    }
}