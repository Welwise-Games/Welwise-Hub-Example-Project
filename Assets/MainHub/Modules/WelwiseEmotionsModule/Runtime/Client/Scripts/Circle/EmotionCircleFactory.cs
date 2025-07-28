using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations.Network.Owner;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Circle.CircleWindow;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts.Circle
{
    public class EmotionsCircleFactory
    {
        private readonly OwnerSelectedEmotionsDataProviderService _ownerSelectedEmotionsDataProviderService;
        private readonly EmotionsConfigProviderService _emotionsConfigProviderService;
        private readonly EmotionsViewConfigProviderService _emotionsViewConfigProviderService;
        private readonly EmotionsViewFactory _emotionsViewFactory;
        private readonly IAssetLoader _assetLoader;

        private readonly Container _container = new Container();

        private const string EmotionsCircleWindowAssetId =
#if ADDRESSABLES
        "EmotionsCircleWindow";
#else
        "WelwiseEmotionsModule/Runtime/Client/Loadable/EmotionsCircleWindow";
#endif

        public EmotionsCircleFactory(OwnerSelectedEmotionsDataProviderService ownerSelectedEmotionsDataProviderService,
            EmotionsConfigProviderService emotionsConfigProviderService, EmotionsViewFactory emotionsViewFactory,
            EmotionsViewConfigProviderService emotionsViewConfigProviderService, IAssetLoader assetLoader)
        {
            _ownerSelectedEmotionsDataProviderService = ownerSelectedEmotionsDataProviderService;
            _emotionsConfigProviderService = emotionsConfigProviderService;
            _emotionsViewFactory = emotionsViewFactory;
            _emotionsViewConfigProviderService = emotionsViewConfigProviderService;
            _assetLoader = assetLoader;
        }

        public async UniTask DisposeUIAsync()
            => await _container.DestroyAndClearAllImplementationsAsync();

        public async UniTask<EmotionsCircleWindowController> GetEmotionsCircleWindowControllerAsync(
            Transform popupTransform, Func<bool> canSwitchingPopupOpenStateFunc, Func<bool> canDisableCursorOnCloseFunc,
            PlayerEmotionsComponents playerEmotionsComponents) =>
            await _container.GetControllerAsync<EmotionsCircleWindowController, EmotionsCircleWindow>(
                EmotionsCircleWindowAssetId, _assetLoader,
                async window =>
                    _container.RegisterAndGetSingleByType(
                        new EmotionsCircleWindowController(window,
                            await _emotionsViewConfigProviderService.GetEmotionsViewConfig(),
                            _ownerSelectedEmotionsDataProviderService,
                            await _emotionsConfigProviderService.GetEmotionsAnimationsConfig(),
                            (canSwitchingPopupOpenStateFunc ?? (() => true)), canDisableCursorOnCloseFunc,
                            _emotionsViewFactory, playerEmotionsComponents)),
                parent: popupTransform);
    }
}