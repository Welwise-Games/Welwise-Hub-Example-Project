using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseEmotionsModule.Runtime.Client.Scripts;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations.Network.Owner;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network;
using WelwiseItemInShopModule.Client.Scripts;
using WelwiseItemInShopModule.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem.SetEmotions
{
    public class SetEmotionsUIFactory : ISetItemsUIFactory<SelectedEmotionData, SelectedEmotionsData,
        SetItemButtonController<SelectedEmotionData, SelectedEmotionsData>>
    {
        public event Action<SetEmotionsPopupController> CreatedSetEmotionsPopupController;

        private readonly EmotionsConfigProviderService _emotionsConfigProviderService;
        private readonly EmotionsViewConfigProviderService _emotionsViewConfigProviderService;
        private readonly OwnerSelectedEmotionsDataProviderService _ownerSelectedEmotionsDataProviderService;

        public readonly SetItemsUIFactory<SelectedEmotionData, SelectedEmotionsData> SetItemsUIFactory;

        private const string SetEmotionButtonAssetId =
#if ADDRESSABLES
        "SetEmotionButton";
#else
            "WelwiseEmotionsModule/Runtime/Client/Loadable/SetEmotionButton";
#endif

        private const string SetEmotionsPopupAssetId =
#if ADDRESSABLES
        "SetEmotionsPopup";
#else
            "WelwiseEmotionsModule/Runtime/Client/Loadable/SetEmotionsPopup";
#endif


        public SetEmotionsUIFactory(EmotionsConfigProviderService emotionsConfigProviderService,
            EmotionsViewConfigProviderService emotionsViewConfigProviderService,
            OwnerSelectedEmotionsDataProviderService ownerSelectedEmotionsDataProviderService,
            IAssetLoader assetLoader)
        {
            _emotionsConfigProviderService = emotionsConfigProviderService;
            _ownerSelectedEmotionsDataProviderService = ownerSelectedEmotionsDataProviderService;
            SetItemsUIFactory =
                new SetItemsUIFactory<SelectedEmotionData, SelectedEmotionsData>(assetLoader,
                    SetEmotionButtonAssetId,
                    SetEmotionsPopupAssetId);
            _emotionsViewConfigProviderService = emotionsViewConfigProviderService;
        }

        public async UniTask<SetItemButtonController<SelectedEmotionData, SelectedEmotionsData>>
            GetNewSetItemButtonController(
                Transform parent, IItemViewConfig targetEmotionViewConfig, float scaleMultiplierOnBecomeTarget,
                float speedChangingScaleOnSetTargetState)
        {
            return new SetItemButtonController<SelectedEmotionData, SelectedEmotionsData>(
                await SetItemsUIFactory.GetSetItemButtonView(parent),
                targetEmotionViewConfig as EmotionViewConfig,
                _ownerSelectedEmotionsDataProviderService,
                scaleMultiplierOnBecomeTarget, speedChangingScaleOnSetTargetState, LocalizationTablesHolder.Emotions);
        }

        public async UniTask<SetEmotionsPopupController> GetSetEmotionsPopupControllerAsync(Transform popupTransform,
            Transform buttonsParent,
            float scaleMultiplierOnBecomeTarget, float speedChangingScaleOnSetTargetState)
        {
            return await SetItemsUIFactory
                .GetSetItemsPopupControllerAsync(popupTransform,
                    async popup =>
                    {
                        var emotionsConfig = await _emotionsConfigProviderService.GetEmotionsAnimationsConfig();
                        var popupController = new SetEmotionsPopupController(
                            emotionsConfig,
                            await _emotionsViewConfigProviderService.GetEmotionsViewConfig(), this, popup,
                            buttonsParent, new SetEmotionsModel(
                                _ownerSelectedEmotionsDataProviderService, (data) => new SelectedEmotionsData(
                                    data.ToList(), emotionsConfig),
                                (ordinalIndex, index) => new SelectedEmotionData(ordinalIndex, index)),
                            scaleMultiplierOnBecomeTarget, speedChangingScaleOnSetTargetState);

                        CreatedSetEmotionsPopupController?.Invoke(popupController);
                        return popupController;
                    });
        }
    }
}