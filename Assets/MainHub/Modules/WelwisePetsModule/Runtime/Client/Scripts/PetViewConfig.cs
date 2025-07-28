using UnityEngine;
using UnityEngine.AddressableAssets;
using WelwiseItemInShopModule.Client.Scripts;

namespace WelwisePetsModule.Runtime.Client.Scripts
{
    [CreateAssetMenu(menuName = "WelwisePetsModule/PetViewConfig")]
    public class PetViewConfig : ScriptableObject, IItemViewConfig
    {
        [field: SerializeField] public string ItemIndex { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
#if !ADDRESSABLES || UNITY_EDITOR
        [field: SerializeField] public GameObject Prefab { get; private set; }
#endif
        [field: SerializeField] public AssetReference PrefabReference { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }

        [field: SerializeField]
        [field: Range(0.1f, 100f)]
        public float Speed { get; private set; } = 5f;
    }
}