using Cysharp.Threading.Tasks;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.TrainingSystem
{
    public class TrainingConfigsProviderService
    {
        private readonly IAssetLoader _assetLoader;
        private readonly Container _container = new Container();

        private const string ArrowsDisplayingConfigAssetId = 
#if ADDRESSABLES
        "ArrowsDisplayingConfig";
#else
        "WelwiseHubExampleModule/Runtime/Client/Loadable/Configs/ArrowsDisplayingConfig";
#endif

        public TrainingConfigsProviderService(IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }

        public async UniTask<ArrowsDisplayingConfig> GetArrowsDisplayingConfigAsync() =>
            await _container.GetOrLoadAndRegisterObjectAsync<ArrowsDisplayingConfig>(
                ArrowsDisplayingConfigAssetId, _assetLoader);
    }
}