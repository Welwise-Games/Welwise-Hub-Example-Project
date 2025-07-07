using Cysharp.Threading.Tasks;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.EventBusSystem;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.UISystem
{
    public class LoadingUIFactory
    {
        private readonly EventBus _eventBus;
        private readonly IAssetLoader _assetLoader;

        private readonly Container _container = new Container();

        private const string LoadingGamePopupAssetId = 
#if ADDRESSABLES
        "LoadingGamePopup";
#else
        "WelwiseHubExampleModule/Runtime/Client/Loadable/Prefabs/LoadingGamePopup";
#endif

        public LoadingUIFactory(EventBus eventBus, IAssetLoader assetLoader)
        {
            _eventBus = eventBus;
            _assetLoader = assetLoader;
        }

        public async UniTask<LoadingGamePopupController> GetLoadingGamePopupControllerAsync()
            => await _container.GetControllerAsync<LoadingGamePopupController, LoadingGamePopup>(
                LoadingGamePopupAssetId, _assetLoader,
                async popup =>
                {
                    _container.RegisterAndGetSingleByType(new LoadingGamePopupController(popup, _eventBus));
                }, shouldMakeDontDestroyOnLoad: true);
    }
}