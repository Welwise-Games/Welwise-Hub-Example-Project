using System.Collections.Generic;
using FishNet.Serializing;

namespace WelwiseItemInShopModule.Shared.Scripts
{
    public interface IClientSelectedItemsData<TSelectedItemData> where TSelectedItemData : ISelectedItemData
    {
        List<TSelectedItemData> SelectedItemsData { get; set; }
    }
}