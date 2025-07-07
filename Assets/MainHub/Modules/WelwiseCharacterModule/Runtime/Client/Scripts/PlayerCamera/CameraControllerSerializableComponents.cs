using UnityEngine;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.PlayerCamera
{
    public class CameraControllerSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public CameraConfig CameraConfig { get; private set; }
        [field: SerializeField] public MonoBehaviourObserver MonoBehaviourObserver { get; private set; }
    }
}