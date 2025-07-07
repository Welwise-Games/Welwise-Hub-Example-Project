using Cysharp.Threading.Tasks;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseChangingClothesModule.Runtime.Shared.Scripts
{
    public class ClientsConfigsProviderService
    {
        private readonly IAssetLoader _assetProvider;
        private readonly Container _container = new Container();

        private const string ClientsConfigAssetId =
#if ADDRESSABLES
        "ClientsConfig";
#else
        "WelwiseHubExampleModule/Runtime/Client/Loadable/Configs/ClientsConfig";
#endif

        public ClientsConfigsProviderService(IAssetLoader assetProvider)
        {
            _assetProvider = assetProvider;
        }


        public async UniTask<ClientsConfig> GetClientsConfigAsync() =>
            await _container.GetOrLoadAndRegisterObjectAsync<ClientsConfig>(
                ClientsConfigAssetId, _assetProvider);
    }
}