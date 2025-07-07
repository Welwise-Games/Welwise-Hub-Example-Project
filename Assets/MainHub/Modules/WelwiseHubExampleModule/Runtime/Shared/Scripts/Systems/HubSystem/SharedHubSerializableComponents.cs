using UnityEngine;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts.Systems.HubSystem
{
    public class SharedHubSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public Transform[] SetPlayerAnimationPlacesTransforms { get; private set; }
        [field: SerializeField] public Transform[] PortalsTransforms { get; private set; }
        [field: SerializeField] public Transform ShopTransform { get; private set; }
    }
}