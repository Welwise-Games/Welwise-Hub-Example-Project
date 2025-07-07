using Cysharp.Threading.Tasks;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseEmotionsModule.Runtime.Shared.Scripts
{
    public class EmotionsConfigsProviderService
    {
        private readonly IAssetLoader _assetLoader;
        private readonly Container _container = new Container();

        private const string EmotionsAnimationsConfigAssetId = 
#if ADDRESSABLES
        "EmotionsAnimationsConfig";
#else
        "WelwiseEmotionsModule/Runtime/Shared/Loadable/Configs/EmotionsAnimationsConfig";
#endif

        public EmotionsConfigsProviderService(IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }

        public async UniTask<EmotionsAnimationsConfig> GetEmotionsAnimationsConfig() =>
            await _container.GetOrLoadAndRegisterObjectAsync<EmotionsAnimationsConfig>(EmotionsAnimationsConfigAssetId, _assetLoader);
    }
}