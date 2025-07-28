using UnityEngine;
using WelwiseItemInShopModule.Shared.Scripts;

namespace WelwisePetsModule.Runtime.Shared.Scripts
{
    [CreateAssetMenu(menuName = "WelwisePetsModule/PetConfig")]
    public class PetConfig : ScriptableObject, IIndexableItemConfig
    {
        [field: SerializeField] public string Index { get; private set; }
    }
}