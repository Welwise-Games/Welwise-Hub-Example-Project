using Cysharp.Threading.Tasks;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts
{
    public class EmotionsViewConfigsProviderService
    {
        private readonly IAssetLoader _assetLoader;
        private readonly Container _container = new Container();

        private const string EmotionsViewConfigAssetId = 
#if ADDRESSABLES
        "EmotionsViewConfig";
#else
        "WelwiseEmotionsModule/Runtime/Client/Loadable/EmotionsViewConfig";
#endif
        
        public EmotionsViewConfigsProviderService(IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }

        public async UniTask<EmotionsViewConfig> GetEmotionsViewConfigAsync() =>
            await _container.GetOrLoadAndRegisterObjectAsync<EmotionsViewConfig>(EmotionsViewConfigAssetId, _assetLoader);
    }
}