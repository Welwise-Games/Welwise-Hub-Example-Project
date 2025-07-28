using Cysharp.Threading.Tasks;
using WelwiseItemInShopModule.Client.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts
{
    public class EmotionsViewConfigProviderService
    {
        private readonly IAssetLoader _assetLoader;
        private readonly Container _container = new Container();
        
        private const string EmotionsViewConfigAssetId =
#if ADDRESSABLES
        "EmotionsViewConfig";
#else
            "WelwiseEmotionsModule/Runtime/Client/Loadable/EmotionsViewConfig";
#endif

        public EmotionsViewConfigProviderService(IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }
        
        public async UniTask<EmotionsViewConfig> GetEmotionsViewConfig() =>
            await _container.GetOrLoadAndRegisterObjectAsync<EmotionsViewConfig>(EmotionsViewConfigAssetId, _assetLoader);
    }
}