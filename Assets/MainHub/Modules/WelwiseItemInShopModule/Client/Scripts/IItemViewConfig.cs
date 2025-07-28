using UnityEngine;
using WelwiseItemInShopModule.Shared.Scripts;

namespace WelwiseItemInShopModule.Client.Scripts
{
    public interface IItemViewConfig
    {
        string ItemIndex { get; }
        Sprite Sprite { get; }
    }
}