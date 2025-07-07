using System;
using System.Collections.Generic;
using System.Linq;
using FishNet;
using UnityEngine;
using UnityEngine.EventSystems;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations.Network.Owner;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network;
using WelwiseSharedModule.Runtime.Client.Scripts.Localization;
using WelwiseSharedModule.Runtime.Client.Scripts.Tools;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts.Circle.CircleWindow
{
    public class EmotionsCircleWindowController
    {
        private readonly EmotionsCircleWindow _window;
        private readonly EmotionsViewConfig _emotionsViewConfig;
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
            _window = window;
            _emotionsViewConfig = emotionsViewConfig;
            _ownerSelectedEmotionsDataProviderService = ownerSelectedEmotionsDataProviderService;
            _emotionsViewFactory = emotionsViewFactory;
            _ownerSelectedEmotionsDataProviderService.UpdatedEmotionsData += FillPopupButtonsEmotionImageSprites;
            
            _startButtonsColor = window.PlayEmotionButtons.First().Button.image.color;

            SubscribePlayingEmotionButtons(playerEmotionsComponents, emotionsAnimationsConfig);
            TryCreatingInputHandlerAndSubscribeOpenPopupButton(canSwitchingPopupOpenStateFunc,
                canDisableCursorOnCloseFunc);

            _window.OpenCircleKeyCodeParent.gameObject.SetActive(!DeviceDetectorTools.IsMobile());

            window.Popup.Opened += FillPopupButtonsEmotionImagesSprites;

            MarkEmotionButtonAsTarget(0);

            void FillPopupButtonsEmotionImageSprites(List<SelectedEmotionData> selectedEmotionData)
            {
                selectedEmotionData.ForEach(emotionData => FillPopupButtonEmotionImageSpriteAsync(
                    window, emotionData.IndexInsideCircle));
            }
        }

        private async void FillPopupButtonEmotionImageSpriteAsync(EmotionsCircleWindow window, int buttonIndex)
        {
            var emotionIndex = _ownerSelectedEmotionsDataProviderService.GetEmotionDataByCircleIndex(buttonIndex)
                ?.EmotionIndex;

            var button = window.PlayEmotionButtons.SafeGet(buttonIndex);

            if (!button)
                return;

            var isEmotionIndexNull = emotionIndex == null;

            button.EmotionImage.gameObject.SetActive(!isEmotionIndexNull);

            if (isEmotionIndexNull)
                return;

            var emotionConfig =
                _emotionsViewConfig.EmotionsConfigs.FirstOrDefault(config => config.EmotionIndex == emotionIndex);

            button.EmotionImage.sprite = emotionConfig?.Sprite;
        }

        private void FillPopupButtonsEmotionImagesSprites()
        {
            for (var i = 0; i < _window.PlayEmotionButtons.Length; i++)
                FillPopupButtonEmotionImageSpriteAsync(_window, i);
        }

        private void TryCreatingInputHandlerAndSubscribeOpenPopupButton(
            Func<bool> canSwitchingPopupOpenStateFunc, Func<bool> canDisableCursorOnCloseFunc)
        {
            _window.SetOpenStateButton.onClick.AddListener(() => _window.Popup.TrySettingOpenState());

            if (DeviceDetectorTools.IsMobile()) return;

            _window.OpenCircleKeyCodeText.text = _emotionsViewConfig.OpenCircleKeycodes.FirstOrDefault().ToString();
            new EmotionsCircleWindowStandaloneInputHandler(_window, _emotionsViewConfig, canSwitchingPopupOpenStateFunc,
                canDisableCursorOnCloseFunc);
        }

        private void SubscribePlayingEmotionButtons(PlayerEmotionsComponents playerEmotionsComponents,
            EmotionsAnimationsConfig emotionsAnimationsConfig)
        {
            for (var i = 0; i < _window.PlayEmotionButtons.Length; i++)
            {
                var button = _window.PlayEmotionButtons[i];
                var buttonIndex = i;
                button.RaycastableImage.alphaHitTestMinimumThreshold = _window.CirclePartAlphaHitTestMinimumThreshold;
                button.Button.onClick.AddListener(() => _window.Popup.TryClosing());
                button.Button.onClick.AddListener(() =>
                    TryPlayingEmotionAsync(playerEmotionsComponents, buttonIndex,
                        emotionsAnimationsConfig));

                button.PointerEnterExitObserver.EnteredWithoutArgs += () =>
                {
                    DrawAllButtonsImagesWithDefaultColor();
                    MarkEmotionButtonAsTarget(buttonIndex);
                };
            }
        }

        private void MarkEmotionButtonAsTarget(int buttonIndex)
        {
            RotateTargetEmotionPointerImage(buttonIndex);
            DrawButtonImageWithTargetButtonColor(buttonIndex);
            UpdateTargetEmotionNameTextAsync(buttonIndex);
        }

        private async void UpdateTargetEmotionNameTextAsync(int buttonIndex)
        {
            var emotionIndex = _ownerSelectedEmotionsDataProviderService.GetEmotionDataByCircleIndex(buttonIndex)
                ?.EmotionIndex;

            var emotionViewConfigName = emotionIndex != null
                ? _emotionsViewConfig.EmotionsConfigs
                    .FirstOrDefault(config => config.EmotionIndex == emotionIndex)?.Name
                : null;

            _window.TargetEmotionNameText.text =
                emotionIndex == null || emotionViewConfigName == null
                    ? await LocalizationTools.GetLocalizedStringAsync(LocalizationTablesHolder.EmotionsCircleWindow,
                        LocalizationKeysHolder.DanceVinyl)
                    : await LocalizationTools.GetLocalizedStringAsync(LocalizationTablesHolder.Emotions,
                        emotionViewConfigName);
        }

        private void DrawAllButtonsImagesWithDefaultColor() =>
            _window.PlayEmotionButtons.ForEach(button => button.CirclePartImage.color = _startButtonsColor);

        private void DrawButtonImageWithTargetButtonColor(int buttonIndex) =>
            _window.PlayEmotionButtons[buttonIndex].CirclePartImage.color = _window.SelectedPlayEmotionButtonsColor;

        private void RotateTargetEmotionPointerImage(int buttonIndex)
        {
            var startRotation = _window.TargetEmotionPointerImage.transform.rotation.eulerAngles;
            _window.TargetEmotionPointerImage.transform.rotation = Quaternion.Euler(new Vector3(startRotation.x,
                startRotation.y, 180 - buttonIndex * 45));
        }

        private async void TryPlayingEmotionAsync(PlayerEmotionsComponents playerEmotionsComponents, int buttonIndex,
            EmotionsAnimationsConfig emotionsAnimationsConfig)
        {
            var emotionIndex = _ownerSelectedEmotionsDataProviderService.GetEmotionDataByCircleIndex(buttonIndex)
                ?.EmotionIndex;

            if (emotionIndex == null)
                return;

            var emotionAnimationConfig = emotionsAnimationsConfig.EmotionsAnimationConfigs.FirstOrDefault(config =>
                config.EmotionIndex == emotionIndex);
            
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