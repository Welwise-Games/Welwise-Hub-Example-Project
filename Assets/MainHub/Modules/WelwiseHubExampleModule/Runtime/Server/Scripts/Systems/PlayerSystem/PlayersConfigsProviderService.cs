using Cysharp.Threading.Tasks;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseHubExampleModule.Runtime.Server.Scripts.Systems.PlayerSystem
{
    public class PlayersConfigsProviderService
    {
        private readonly IAssetLoader _assetLoader;
        private readonly Container _container = new Container();

        private const string PlayersConfigAssetId =
#if ADDRESSABLES
        "PlayersConfig";
#else
        "WelwiseHubExampleModule/Runtime/Server/Loadable/PlayersConfig";
#endif
        
        public PlayersConfigsProviderService(IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }

        public async UniTask<PlayersConfig> GetPlayersConfigAsync() =>
            await _container.GetOrLoadAndRegisterObjectAsync<PlayersConfig>(PlayersConfigAssetId, _assetLoader);
    }
}