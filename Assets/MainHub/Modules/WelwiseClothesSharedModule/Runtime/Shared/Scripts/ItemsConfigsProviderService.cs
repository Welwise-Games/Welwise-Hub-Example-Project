using Cysharp.Threading.Tasks;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseClothesSharedModule.Runtime.Shared.Scripts
{
    public class ItemsConfigsProviderService
    {
        private readonly IAssetLoader _assetLoader;
        private readonly Container _container;

        private const string ItemsConfigsAssetId = 
#if ADDRESSABLES
            "ItemsConfig";
#else
            "WelwiseClothesSharedModule/Runtime/Shared/Loadable/ItemsConfig";
#endif
        
        public ItemsConfigsProviderService(IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
            _container = new Container();
        }

        public async UniTask<ItemsConfig> GetItemsConfigAsync() =>
            await _container.GetOrLoadAndRegisterObjectAsync<ItemsConfig>(
                ItemsConfigsAssetId, _assetLoader);
    }
}