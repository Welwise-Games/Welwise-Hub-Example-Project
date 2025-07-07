using Cysharp.Threading.Tasks;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.HubSystem;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure.Services
{
    public class ClientConfigsProviderService
    {
        private readonly IAssetLoader _assetLoader;
        private readonly Container _container = new Container();

        private const string PortalsConfig = 
#if ADDRESSABLES
        "PortalsConfig";
#else
        "WelwiseHubExampleModule/Runtime/Client/Loadable/Configs/PortalsConfig";
#endif

        public ClientConfigsProviderService(IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }

        public async UniTask<PortalsConfig> GetPortalsConfigAsync() =>
            await _container.GetOrLoadAndRegisterObjectAsync<PortalsConfig>(
                PortalsConfig, _assetLoader);
    }
}