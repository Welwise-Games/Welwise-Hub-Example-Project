using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseMiniGamesModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace Modules.WelwiseMiniGamesModule.Runtime.Main.Scripts
{
    public class MiniGamesFactory
    {
        private readonly Container _container = new Container();
        private readonly IAssetLoader _assetLoader;

        private const string MiniGamesPopupViewAssetId =
#if ADDRESSABLES
            "MiniGamesPopup";
#else
        "WelwiseMiniGamesModule/Runtime/Main/Loadable/MiniGamesPopup";
#endif

        public MiniGamesFactory(IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }

        public async UniTask DisposeAsync() => await _container.DestroyAndClearAllImplementationsAsync();

        public async UniTask<MiniGamesPopupView> GetMiniGamesPopupViewAsync(Transform parent) =>
            await _container.GetOrLoadAndRegisterObjectAsync<MiniGamesPopupView>(MiniGamesPopupViewAssetId, _assetLoader, parent: parent);

        public async UniTask<MiniGameSerializableComponents> GetMiniGameSerializableComponentsInstance(
            MiniGameConfig config)
        {
            var prefab =
#if ADDRESSABLES
                await AssetProvider.LoadAsync<MiniGameSerializableComponents>(
                    await config.PrefabReference.GetAssetIdAsync(), _assetLoader);
#else
                config.Prefab;
#endif

            return UnityEngine.Object.Instantiate(prefab);
        }
    }
}