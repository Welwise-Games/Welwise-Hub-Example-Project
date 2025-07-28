using Cysharp.Threading.Tasks;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwisePetsModule.Runtime.Shared.Scripts
{
    public class PetsConfigProviderService
    {
        private readonly IAssetLoader _assetLoader;
        private readonly Container _container = new Container();

        private const string PetsConfigAssetId
            =
#if ADDRESSABLES
            "PetsConfig";
#else
            "WelwisePetsModule/Runtime/Shared/Loadable/PetsConfig";
#endif
        
        public PetsConfigProviderService(IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }
        
        public async UniTask<PetsConfig> GetPetsConfigAsync() =>
            await _container.GetOrLoadAndRegisterObjectAsync<PetsConfig>(PetsConfigAssetId, _assetLoader);
    }
}