using Cysharp.Threading.Tasks;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.UI
{
    public class UIFactory
    {
        private readonly Container _container = new Container();
        private readonly IAssetLoader _assetLoader;

        private const string UIRootAssetId = 
#if ADDRESSABLES
        "UIRoot";
#else
        "WelwiseHubExampleModule/Runtime/Client/Loadable/Prefabs/UIRoot";
#endif
        
        public UIFactory(IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }

        public async UniTask DisposeUIAsync()
            => await _container.DestroyAndClearAllImplementationsAsync();
        
        public async UniTask<UIRootComponents> GetUIRootAsync() =>
            await _container.GetControllerAsync<UIRootComponents, UIRootSerializableComponents>(
                UIRootAssetId, _assetLoader,
                async root =>
                {
                    _container.RegisterAndGetSingleByType(new UIRootComponents(root,
                        new ErrorTextController(root.ErrorText, root.ErrorTextConfig)));
                });
    }
}