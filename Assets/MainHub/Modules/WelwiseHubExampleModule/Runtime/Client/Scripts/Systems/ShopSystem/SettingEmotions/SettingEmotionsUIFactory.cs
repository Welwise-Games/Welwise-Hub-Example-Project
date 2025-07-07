using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseEmotionsModule.Runtime.Client.Scripts;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations.Network.Owner;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;
using Object = UnityEngine.Object;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem.SettingEmotions
{
    public class SettingEmotionsUIFactory
    {
        public event Action<SettingEmotionsPopupController> CreatedSettingEmotionsPopupController;

        private readonly EmotionsConfigsProviderService _emotionsConfigsProviderService;
        private readonly EmotionsViewConfigsProviderService _emotionsViewConfigsProviderService;
        private readonly OwnerSelectedEmotionsDataProviderService _ownerSelectedEmotionsDataProviderService;

        private readonly IAssetLoader _assetLoader;
        private readonly Container _container = new Container();

        private const string SetEmotionButtonAssetId = 
#if ADDRESSABLES
        "SetEmotionButton";
#else
        "WelwiseEmotionsModule/Runtime/Client/Loadable/SetEmotionButton";
#endif
        
        private const string SettingEmotionsPopupAssetId = 
#if ADDRESSABLES
        "SettingEmotionsPopup";
#else
        "WelwiseEmotionsModule/Runtime/Client/Loadable/SettingEmotionsPopup";
#endif

        public SettingEmotionsUIFactory(EmotionsConfigsProviderService emotionsConfigsProviderService,
            EmotionsViewConfigsProviderService emotionsViewConfigsProviderService,
            OwnerSelectedEmotionsDataProviderService ownerSelectedEmotionsDataProviderService, IAssetLoader assetLoader)
        {
            _emotionsConfigsProviderService = emotionsConfigsProviderService;
            _ownerSelectedEmotionsDataProviderService = ownerSelectedEmotionsDataProviderService;
            _assetLoader = assetLoader;
            _emotionsViewConfigsProviderService = emotionsViewConfigsProviderService;
        }

        public async UniTask<SettingEmotionsPopupController> GetSettingEmotionsPopupControllerAsync(
            Transform popupTransform, Transform buttonsParent,
            float scaleMultiplierOnBecomeTarget,
            float speedChangingScaleOnSetTargetState)
        {
            return await _container.GetControllerAsync<SettingEmotionsPopupController, SettingEmotionsPopup>(
                SettingEmotionsPopupAssetId, _assetLoader,
                async popup =>
                {
                    var popupController = new SettingEmotionsPopupController(
                        await _emotionsConfigsProviderService.GetEmotionsAnimationsConfig(),
                        await _emotionsViewConfigsProviderService.GetEmotionsViewConfigAsync(),
                        this, popup, buttonsParent, new SettingEmotionsModel(
                            _ownerSelectedEmotionsDataProviderService), scaleMultiplierOnBecomeTarget,
                        speedChangingScaleOnSetTargetState);

                    _container.RegisterAndGetSingleByType(popupController);
                    CreatedSettingEmotionsPopupController?.Invoke(popupController);

                    popup.transform.SetSiblingIndex(popupTransform.childCount - 2);
                }, parent: popupTransform);
        }

        public async UniTask<SetEmotionButtonController> GetNewSetEmotionButtonControllerAsync(Transform parent,
            EmotionViewConfig targetEmotionViewConfig, float scaleMultiplierOnBecomeTarget,
            float speedChangingScaleOnSetTargetState)
        {
            var prefab =
                await _container.GetOrLoadAndRegisterObjectAsync<SetEmotionButtonView>(SetEmotionButtonAssetId, _assetLoader,
                    shouldCreate: false);

            var instance = Object.Instantiate(prefab, parent);

            return new SetEmotionButtonController(instance, targetEmotionViewConfig,
                _ownerSelectedEmotionsDataProviderService,
                scaleMultiplierOnBecomeTarget, speedChangingScaleOnSetTargetState);
        }
    }
}