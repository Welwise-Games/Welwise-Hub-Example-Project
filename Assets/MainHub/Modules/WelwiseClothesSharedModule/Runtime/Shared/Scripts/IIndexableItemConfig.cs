using UnityEngine;

namespace WelwiseClothesSharedModule.Runtime.Shared.Scripts
{
    public interface IIndexableItemConfig
    {
        string ItemIndex { get; }
        string ItemName { get; }
        Sprite ItemSprite { get; }
    }
}