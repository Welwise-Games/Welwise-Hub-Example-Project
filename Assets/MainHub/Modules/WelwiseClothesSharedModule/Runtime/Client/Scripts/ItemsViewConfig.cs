using System.Linq;
using UnityEngine;

namespace WelwiseClothesSharedModule.Runtime.Client.Scripts
{
    [CreateAssetMenu(menuName = "WelwiseLoadingClothesModule/ItemsViewConfig")]
    public class ItemsViewConfig : ScriptableObject
    {
        [field: SerializeField] public ItemViewConfig[] Items { get; private set; }
        [field: SerializeField] public Sprite PlayerSkinColorItemSprite { get; private set; }
        [field: SerializeField] public Sprite PlayerDefaultClothesEmissionColorSprite { get; private set; }
        
        public ItemViewConfig TryGettingConfig(string index) => Items.FirstOrDefault(item => item.ItemIndex == index);
    }
}