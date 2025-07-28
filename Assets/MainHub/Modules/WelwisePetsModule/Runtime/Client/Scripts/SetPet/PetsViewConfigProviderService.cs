using Cysharp.Threading.Tasks;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwisePetsModule.Runtime.Client.Scripts.SetPet
{
    public class PetsViewConfigProviderService
    {
        private readonly IAssetLoader _assetLoader;
        private readonly Container _container = new Container();

        private const string PetsViewConfigAssetId
            =
#if ADDRESSABLES
            "PetsViewConfig";
#else
            "WelwisePetsModule/Runtime/Client/Loadable/PetsViewConfig";
#endif

        public PetsViewConfigProviderService(IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }

        public async UniTask<PetsViewConfig> GetPetsViewConfigAsync() =>
            await _container.GetOrLoadAndRegisterObjectAsync<PetsViewConfig>(PetsViewConfigAssetId, _assetLoader);
    }
}