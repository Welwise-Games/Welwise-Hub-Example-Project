using Cysharp.Threading.Tasks;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseClothesSharedModule.Runtime.Client.Scripts
{
    public class ItemsViewConfigsProviderService
    {
        private readonly IAssetLoader _assetLoader;
        private readonly Container _container;

        private const string ItemsViewConfigsAssetId = 
#if ADDRESSABLES
            "ItemsViewConfig";
#else
            "WelwiseClothesSharedModule/Runtime/Client/Loadable/ItemsViewConfig";
#endif
        
        public ItemsViewConfigsProviderService(IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
            _container = new Container();
        }
        
        public async UniTask<ItemsViewConfig> GetItemsViewConfigAsync() =>
            await _container.GetOrLoadAndRegisterObjectAsync<ItemsViewConfig>(
                ItemsViewConfigsAssetId, _assetLoader);
    }
}