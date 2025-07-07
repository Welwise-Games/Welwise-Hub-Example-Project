using Cysharp.Threading.Tasks;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseHubBotsModule.Runtime.Server.Scripts
{
    public class BotsConfigsProviderService
    {
        private readonly IAssetLoader _assetLoader;
        private readonly Container _container = new Container();

        private const string BotsConfigAssetId = 
#if ADDRESSABLES
        "BotsConfig";
#else
        "WelwiseHubBotsModule/Runtime/Server/Loadable/BotsConfig";
#endif
        
        public BotsConfigsProviderService(IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }

        public async UniTask<BotsConfig> GetBotsConfigAsync() =>
            await _container.GetOrLoadAndRegisterObjectAsync<BotsConfig>(BotsConfigAssetId, _assetLoader);
    }
}