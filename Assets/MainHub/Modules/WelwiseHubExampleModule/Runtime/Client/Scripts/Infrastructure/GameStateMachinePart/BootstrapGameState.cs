using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.UISystem;
using WelwiseSharedModule.Runtime.Client.Scripts;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure.GameStateMachinePart
{
    public class BootstrapGameState : IGameState
    {
        private readonly CameraFactory _cameraFactory;
        private readonly LoadingUIFactory _loadingUIFactory;

        public BootstrapGameState(CameraFactory cameraFactory, LoadingUIFactory loadingUIFactory)
        {
            _cameraFactory = cameraFactory;
            _loadingUIFactory = loadingUIFactory;
        }

        public async UniTask EnterAsync()
        {
#if !UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
#endif
            
            await _cameraFactory.GetMainCameraAsync();
            
            _cameraFactory.GetMainCameraAsync().Forget();
            
            var loadingGamePopupController = (await _loadingUIFactory.GetLoadingGamePopupControllerAsync());
            
            loadingGamePopupController.Popup.LoadingSlider.value = 0;

        }

        public async UniTask ExitAsync() {}
    }
}