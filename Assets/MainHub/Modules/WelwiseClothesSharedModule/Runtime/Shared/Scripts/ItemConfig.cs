using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WelwiseClothesSharedModule.Runtime.Shared.Scripts
{
    [CreateAssetMenu(menuName = "WelwiseClothesModule/ItemConfig")]
    public class ItemConfig : ScriptableObject
    {
        [field: SerializeField] public string ItemIndex { get; private set; }
        [field: SerializeField] public ItemCategory ItemCategory { get; private set; }
    }
}