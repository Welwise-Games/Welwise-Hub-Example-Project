using UnityEngine;

namespace WelwiseClothesSharedModule.Runtime.Client.Scripts
{
    public class SkinColorChangerSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public Renderer[] BodyRenderers { get; private set; }
        [field: SerializeField] public Renderer[] ArmsRenderers { get; private set; }
        [field: SerializeField] public Material BodyMaterial { get; private set; }
        [field: SerializeField] public Material ArmsMaterial { get; private set; }
    }
}