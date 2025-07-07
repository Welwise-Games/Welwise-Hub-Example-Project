using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WelwiseClothesSharedModule.Runtime.Shared.Scripts
{
    [CreateAssetMenu(fileName = "ItemConfig", menuName = "ItemConfig", order = 2)]
    public class ItemConfig : ScriptableObject
    {
        [field: SerializeField] public string ItemIndex { get; private set; }
        [field: SerializeField] public ItemCategory ItemCategory { get; private set; }
    }
}