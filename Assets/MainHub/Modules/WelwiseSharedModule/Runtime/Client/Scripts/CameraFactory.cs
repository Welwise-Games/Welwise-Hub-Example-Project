using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseSharedModule.Runtime.Client.Scripts
{
    public class CameraFactory
    {
        private readonly IAssetLoader _assetLoader;
        
        private readonly Container _container = new Container();

        private const string MainCameraAssetId =
#if ADDRESSABLES
        "MainCamera";
#else
        "WelwiseHubExampleModule/Runtime/Client/Loadable/Prefabs/MainCamera";
#endif
        
        public CameraFactory(IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }

        public async UniTask<Camera> GetMainCameraAsync() =>
            await _container.GetOrLoadAndRegisterObjectAsync<Camera>(MainCameraAssetId, _assetLoader,
                shouldMakeDontDestroyOnLoad: true);
    }
}