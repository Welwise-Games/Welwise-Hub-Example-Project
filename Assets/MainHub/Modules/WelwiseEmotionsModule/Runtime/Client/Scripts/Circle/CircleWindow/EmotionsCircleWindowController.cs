using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations.Network.Owner;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network;
using WelwiseSharedModule.Runtime.Client.Scripts.Localization;
using WelwiseUICircleItemModule.Runtime.Scripts;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts.Circle.CircleWindow
{
    public class EmotionsCircleWindowController
    {
        private readonly OwnerSelectedEmotionsDataProviderService _ownerSelectedEmotionsDataProviderService;
        private readonly Color _startButtonsColor;
        private readonly EmotionsViewFactory _emotionsViewFactory;

        public EmotionsCircleWindowController(EmotionsCircleWindow window,
            EmotionsViewConfig emotionsViewConfig,
            OwnerSelectedEmotionsDataProviderService ownerSelectedEmotionsDataProviderService,
            EmotionsAnimationsConfig emotionsAnimationsConfig, Func<bool> canSwitchingPopupOpenStateFunc,
            Func<bool> canDisableCursorOnCloseFunc, EmotionsViewFactory emotionsViewFactory,
            PlayerEmotionsComponents playerEmotionsComponents)
        {
            var emotionsViewConfig1 = emotionsViewConfig;
            _ownerSelectedEmotionsDataProviderService = ownerSelectedEmotionsDataProviderService;

            var itemsCircleWindowController = new ItemsCircleWindowController(window.ItemsCircleWindow,
                canSwitchingPopupOpenStateFunc, canDisableCursorOnCloseFunc, async key =>
                    await LocalizationTools.GetLocalizedStringAsync(LocalizationTablesHolder.Emotions,
                        key), async () => await LocalizationTools.GetLocalizedStringAsync(
                    LocalizationTablesHolder.EmotionsCircleWindow,
                    LocalizationKeysHolder.DanceVinyl), () => emotionsViewConfig1.OpenCircleKeycodes, buttonIndex =>
                    _ownerSelectedEmotionsDataProviderService.GetItemDataByOrdinalIndex(buttonIndex)
                        ?.Index, itemIndex => emotionsViewConfig1.Configs
                    .FirstOrDefault(config => config.ItemIndex == itemIndex)
                    ?.Sprite,
                itemIndex => emotionsViewConfig1.Configs.FirstOrDefault(config => config.ItemIndex == itemIndex)
                    ?.Name);

            _ownerSelectedEmotionsDataProviderService.UpdatedItemData += FillPopupButtonsEmotionImageSprites;
            _emotionsViewFactory = emotionsViewFactory;

            itemsCircleWindowController.ButtonClicked += ClosePopupAndTryPlayingEmotion;

            void ClosePopupAndTryPlayingEmotion(int buttonIndex)
            {
                window.ItemsCircleWindow.Popup.TryClosing();
                TryPlayingEmotionAsync(playerEmotionsComponents, buttonIndex,
                    emotionsAnimationsConfig);
            }

            void FillPopupButtonsEmotionImageSprites(List<SelectedEmotionData> selectedEmotionData)
            {
                selectedEmotionData.ForEach(emotionData =>
                    itemsCircleWindowController.FillPopupButtonItemImageSprite(emotionData.OrdinalIndex));
            }
        }

        private async void TryPlayingEmotionAsync(PlayerEmotionsComponents playerEmotionsComponents, int buttonIndex,
            EmotionsAnimationsConfig emotionsAnimationsConfig)
        {
            var emotionIndex = _ownerSelectedEmotionsDataProviderService.GetItemDataByOrdinalIndex(buttonIndex)
                ?.Index;

            if (emotionIndex == null)
                return;

            var emotionAnimationConfig = emotionsAnimationsConfig.Configs.FirstOrDefault(config =>
                config.Index == emotionIndex);

            if (emotionAnimationConfig != null && emotionAnimationConfig.OverrideController)
            {
                var particlesParents = await _emotionsViewFactory.TryCreatingParticlesParentsAsync(
                    playerEmotionsComponents.ParticleEventController.transform, emotionIndex);

                playerEmotionsComponents.ParticleEventController.UpdateParticleObjects(
                    particlesParents.Select(parent => parent.gameObject).ToArray());

                playerEmotionsComponents?.EmotionsAnimatorController
                    .SetAnimatorControllerAndTryStartingEmotionAnimation(
                        emotionAnimationConfig.OverrideController, emotionIndex, buttonIndex);
            }
        }
    }
}