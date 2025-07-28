using Cysharp.Threading.Tasks;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace Modules.WelwiseMiniGamesModule.Runtime.Main.Scripts
{
    public class MiniGamesConfigProviderService
    {
        private readonly Container _container = new Container();
        private readonly IAssetLoader _assetLoader;

        private const string MiniGamesConfigAssetId =
#if ADDRESSABLES
            "MiniGamesConfig";
#else
            "WelwiseMiniGamesModule/Runtime/Shared/Loadable/MiniGamesConfip";
#endif

        public MiniGamesConfigProviderService(IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }

        public async UniTask<MiniGamesConfig> GetMiniGamesConfigAsync() =>
            await _container.GetOrLoadAndRegisterObjectAsync<MiniGamesConfig>(MiniGamesConfigAssetId, _assetLoader);
    }
}