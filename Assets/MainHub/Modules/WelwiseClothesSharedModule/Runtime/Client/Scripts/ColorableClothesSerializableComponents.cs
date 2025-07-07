using UnityEngine;

namespace WelwiseClothesSharedModule.Runtime.Client.Scripts
{
    public class ColorableClothesSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public Renderer RendererWithMaterials { get; private set; }
    }
}