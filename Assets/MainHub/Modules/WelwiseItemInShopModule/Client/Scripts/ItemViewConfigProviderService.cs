using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseItemInShopModule.Client.Scripts
{
    public class ItemViewConfigProviderService<TItemsViewConfig, TItemViewConfig>
        where TItemsViewConfig : Object, IItemsViewConfig<TItemViewConfig>
        where TItemViewConfig : IItemViewConfig
    {
        private readonly IAssetLoader _assetLoader;
        private readonly Container _container = new Container();

        private readonly string _itemConfigAssetId;

        public ItemViewConfigProviderService(IAssetLoader assetLoader, string itemConfigAssetId)
        {
            _assetLoader = assetLoader;
            _itemConfigAssetId = itemConfigAssetId;
        }

        public async UniTask<TItemsViewConfig> GetViewConfig() =>
            await _container.GetOrLoadAndRegisterObjectAsync<TItemsViewConfig>(_itemConfigAssetId, _assetLoader);
    }
}