using UnityEngine;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts.Systems.ShopSystem
{
    public class PersistentColorableItem : IIndexableItemConfig
    {
        public string ItemIndex { get; }
        public string ItemName { get; }
        public Sprite ItemSprite { get; }
        
        public PersistentColorableItem(string index, Sprite itemSprite, string itemName)
        {
            ItemIndex = index;
            ItemSprite = itemSprite;
            ItemName = itemName;
        }
    }
}