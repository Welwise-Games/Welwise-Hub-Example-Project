using UnityEngine;
using WelwiseClothesSharedModule.Runtime.Client.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem
{
    public class ShopSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public SkinColorChangerSerializableComponents PlayerPreviewSkinColorChangerSerializableComponents { get; private set; }
        [field: SerializeField] public ColorableClothesViewSerializableComponents ColorableClothesViewSerializableComponents { get; private set; }
        [field: SerializeField] public ColliderObserver OpenShopColliderObserver { get; private set; }
        [field: SerializeField] public Transform PlayerPreviewTransform { get; private set; }
        [field: SerializeField] public float PlayerPreviewRotationSensitivity { get; private set; } = 1;
        [field: SerializeField] public Transform PositionProviderTransform { get; private set; }
    }
}