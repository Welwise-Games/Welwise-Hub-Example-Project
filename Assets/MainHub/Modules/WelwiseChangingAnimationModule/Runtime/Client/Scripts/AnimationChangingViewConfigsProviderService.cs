using Cysharp.Threading.Tasks;
using WelwiseChangingAnimationModule.Runtime.Client.Scripts.SetPlayerAnimationsButton;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseChangingAnimationModule.Runtime.Client.Scripts
{
    public class AnimationChangingViewConfigsProviderService
    {
        private readonly IAssetLoader _assetLoader;
        private readonly Container _container = new Container();

        private const string SetPlayerAnimationsAndPositionButtonsConfigAssetId = 
#if ADDRESSABLES
            "SetPlayerAnimationsAndPositionButtonsConfig";
#else
            "WelwiseHubExampleModule/Runtime/Client/Loadable/Configs/SetPlayerAnimationsAndPositionButtonsConfig";
#endif
        
        public AnimationChangingViewConfigsProviderService(IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }
        
        public async UniTask<SetPlayerAnimationsButtonsConfig> GetSetPlayerAnimationsAndPositionButtonsAsync() =>
            await _container.GetOrLoadAndRegisterObjectAsync<SetPlayerAnimationsButtonsConfig>(
                SetPlayerAnimationsAndPositionButtonsConfigAssetId, _assetLoader);
    }
}