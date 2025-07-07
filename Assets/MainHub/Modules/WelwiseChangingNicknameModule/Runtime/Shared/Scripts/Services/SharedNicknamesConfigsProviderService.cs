using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Services
{
    public class SharedNicknamesConfigsProviderService
    {
        private readonly IAssetLoader _assetProvider;
        private readonly Container _container = new Container();

        private const string SharedClientsNicknamesConfigAssetId =
#if ADDRESSABLES
        "SharedClientsNicknamesConfig";
#else
        "WelwiseChangingNicknameModule/Runtime/Shared/Loadable/SharedClientsNicknamesConfig";
#endif

        public SharedNicknamesConfigsProviderService(IAssetLoader assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public async UniTask<SharedClientsNicknamesConfig> GetSharedClientsNicknamesConfigAsync()
            => await _container.GetOrLoadAndRegisterObjectAsync<SharedClientsNicknamesConfig>(
                SharedClientsNicknamesConfigAssetId, _assetProvider);
    }
}