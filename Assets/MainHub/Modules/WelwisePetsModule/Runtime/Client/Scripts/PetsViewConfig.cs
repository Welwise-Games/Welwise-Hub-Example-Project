using System.Linq;
using UnityEngine;
using WelwiseItemInShopModule.Client.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwisePetsModule.Runtime.Client.Scripts
{
    [CreateAssetMenu(menuName = "WelwisePetsModule/PetsViewConfig")]
    public class PetsViewConfig : ScriptableObject, IItemsViewConfig<PetViewConfig>
    {
        [field: SerializeField] public PetViewConfig[] Configs { get; private set; }
        [field: SerializeField] public ErrorTextConfig ErrorTextConfig { get; private set; }
        [field: SerializeField] public Vector3[] PetOffsetFromPetOwnerByOrdinalIndex { get; private set; }

        public PetViewConfig TryGettingPetViewConfigByIndex(string id)
            => Configs.FirstOrDefault(config => config.ItemIndex == id);
    }
}