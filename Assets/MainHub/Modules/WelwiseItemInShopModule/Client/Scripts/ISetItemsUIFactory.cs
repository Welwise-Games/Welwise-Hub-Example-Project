using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseItemInShopModule.Shared.Scripts;

namespace WelwiseItemInShopModule.Client.Scripts
{
    public interface ISetItemsUIFactory<out TSelectedItemData, out TClientSelectedItemsData, TSetItemButtonController>
        where TSelectedItemData : class, ISelectedItemData
        where TClientSelectedItemsData : IClientSelectedItemsData<TSelectedItemData>
        where TSetItemButtonController : SetItemButtonController<TSelectedItemData, TClientSelectedItemsData>
    {
        UniTask<TSetItemButtonController> GetNewSetItemButtonController(Transform parent, IItemViewConfig viewConfig,
                float scaleMultiplierOnBecomeTarget,
                float speedChangingScaleOnSetTargetState);
    }
}