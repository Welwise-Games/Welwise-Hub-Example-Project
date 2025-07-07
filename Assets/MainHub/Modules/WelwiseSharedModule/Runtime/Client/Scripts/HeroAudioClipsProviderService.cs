using Cysharp.Threading.Tasks;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseSharedModule.Runtime.Client.Scripts
{
    public class HeroAudioClipsProviderService
    {
        private readonly Container _container = new Container();
        private readonly IAssetLoader _assetProvider;

        private const string HeroAudioClipsConfigAssetId =
#if ADDRESSABLES
        "HeroAudioClipsConfig";
#else
        "WelwiseHubExampleModule/Runtime/Client/Loadable/Configs/HeroAudioClipsConfig";
#endif

        public HeroAudioClipsProviderService(IAssetLoader assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public async UniTask<HeroAudioClipsConfig> GetHeroAudioClipsConfigAsync() =>
            await _container.GetOrLoadAndRegisterObjectAsync<HeroAudioClipsConfig>(HeroAudioClipsConfigAssetId,
                _assetProvider);
    }
}