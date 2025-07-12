using System.Linq;
using UnityEngine;

namespace WelwiseClothesSharedModule.Runtime.Shared.Scripts
{
    [CreateAssetMenu(fileName = "ItemsConfig", menuName = "WelwiseLoadingClothesModule/ItemsConfig")]
    public class ItemsConfig : ScriptableObject
    {
        [field: SerializeField] public ItemConfig[] Items { get; private set; }
        public ItemConfig TryGettingConfig(string index) => Items.FirstOrDefault(item => item.ItemIndex == index);
    }
}