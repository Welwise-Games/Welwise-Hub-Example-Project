using UnityEngine;
using UnityEngine.AddressableAssets;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;

namespace WelwiseClothesSharedModule.Runtime.Client.Scripts
{
    [CreateAssetMenu(menuName = "WelwiseClothesModule/ItemViewConfig", order = 2)]
    public class ItemViewConfig : ScriptableObject, IIndexableItemConfig
    {
        [field: SerializeField] public string ItemIndex { get; private set; }
        [field: SerializeField] public Sprite ItemSprite { get; private set; }
        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public AssetReference PrefabReference { get; private set; }
#if !ADDRESSABLES || UNITY_EDITOR
        [field: SerializeField] public ColorableClothesSerializableComponents Prefab { get; private set; }
#endif
    }
}