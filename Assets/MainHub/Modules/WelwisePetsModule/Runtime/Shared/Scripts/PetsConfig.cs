using System.Linq;
using UnityEngine;
using WelwiseItemInShopModule.Shared.Scripts;

namespace WelwisePetsModule.Runtime.Shared.Scripts
{
    [CreateAssetMenu(menuName = "WelwisePetsModule/PetsConfig")]
    public class PetsConfig : ScriptableObject, IItemsConfig<PetConfig>
    {
        [field: SerializeField] [field: Range(1, 10)] public int MaxSelectedItemsNumber { get; private set; }
        [field: SerializeField] public PetConfig[] Configs { get; private set; }

        public PetConfig TryGettingPetViewConfigByIndex(string id)
            => Configs.FirstOrDefault(config => config.Index == id);
    }
}