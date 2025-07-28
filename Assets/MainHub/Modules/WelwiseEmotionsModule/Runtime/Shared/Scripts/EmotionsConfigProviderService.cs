using Cysharp.Threading.Tasks;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations;
using WelwiseItemInShopModule.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseEmotionsModule.Runtime.Shared.Scripts
{
    public class EmotionsConfigProviderService
    {
        private const string EmotionsAnimationsConfigAssetId =
#if ADDRESSABLES
        "EmotionsAnimationsConfig";
#else
            "WelwiseEmotionsModule/Runtime/Shared/Loadable/Configs/EmotionsAnimationsConfig";
#endif

        private readonly IAssetLoader _assetLoader;
        private readonly Container _container = new Container();

        public EmotionsConfigProviderService(IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }

        public async UniTask<EmotionsAnimationsConfig> GetEmotionsAnimationsConfig() =>
            await _container.GetOrLoadAndRegisterObjectAsync<EmotionsAnimationsConfig>(EmotionsAnimationsConfigAssetId,
                _assetLoader);
    }
}