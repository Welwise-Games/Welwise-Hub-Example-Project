using Cysharp.Threading.Tasks;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.InputServices
{
    public class InputConfigProviderService
    {
        private readonly Container _container = new Container();
        private readonly IAssetLoader _assetLoader;

        public InputConfigProviderService(IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }

        private const string InputConfigAssetId =
#if ADDRESSABLES
            "InputConfig";
#else
            "WelwiseCharacterModule/Runtime/Client/Loadable/InputConfig";
#endif

        public void Dispose() => _container.DestroyAndClearAllImplementationsAsync().Forget();
        public async UniTask<InputConfig> GetInputConfigAsync() => await _container.GetOrLoadAndRegisterObjectAsync<InputConfig>(InputConfigAssetId, _assetLoader);
    }
}