using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WelwiseEmotionsModule.Runtime.Client.Scripts;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations;
using WelwiseSharedModule.Runtime.Client.Scripts.Localization;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem.SettingEmotions
{
    public class SettingEmotionsPopupController
    {
        public event Action ChangedButtonSelectedMode;

        public readonly SettingEmotionsPopup SettingPopup;
        public readonly SettingEmotionsModel SettingEmotionsModel;

        private readonly List<SetEmotionButtonController> _buttonsControllers =
            new List<SetEmotionButtonController>();

        private readonly EmotionsAnimationsConfig _emotionsAnimationsConfig;
        private readonly EmotionsViewConfig _emotionsViewConfig;
        private readonly SettingEmotionsUIFactory _settingEmotionsUiFactory;

        private readonly Transform _buttonsParent;
        private readonly ErrorTextController _errorTextController;
        private readonly float _settingButtonsScaleMultiplierOnBecomeTarget;
        private readonly float _settingButtonsSpeedChangingScaleOnSetTargetState;

        public SettingEmotionsPopupController(EmotionsAnimationsConfig emotionsAnimationsConfig,
            EmotionsViewConfig emotionsViewConfig, SettingEmotionsUIFactory settingEmotionsUiFactory,
            SettingEmotionsPopup popup, Transform buttonsParent,
            SettingEmotionsModel settingEmotionsModel, float settingButtonsScaleMultiplierOnBecomeTarget, float settingButtonsSpeedChangingScaleOnSetTargetState)
        {
            SettingPopup = popup;
            SettingEmotionsModel = settingEmotionsModel;
            _settingButtonsScaleMultiplierOnBecomeTarget = settingButtonsScaleMultiplierOnBecomeTarget;
            _settingButtonsSpeedChangingScaleOnSetTargetState = settingButtonsSpeedChangingScaleOnSetTargetState;
            _emotionsAnimationsConfig = emotionsAnimationsConfig;
            _emotionsViewConfig = emotionsViewConfig;
            _settingEmotionsUiFactory = settingEmotionsUiFactory;
            _errorTextController =
                new ErrorTextController(popup.TooMuchSelectedEmotionsText, emotionsViewConfig.ErrorTextConfig);
            _buttonsParent = buttonsParent;

            popup.ClearSelectedEmotions.onClick.AddListener(MakeAllButtonsUnselected);
        }

        public void InitializeOnOpen()
        {
            SettingEmotionsModel.RevertToSavedClientEmotionsData();
            RecreateButtons();
        }

        public void DeInitializeOnClose() => DestroyAllButtons();

        private void MakeAllButtonsUnselected() =>
            _buttonsControllers.ForEach(button =>
                button.TrySettingUnselectedModeAndUpdateView());

        public void ReturnLastSavedValuesAndUpdateView()
        {
            SettingEmotionsModel.RevertToSavedClientEmotionsData();
            _buttonsControllers.ForEach(button => button.ReturnLastSavedValuesAndUpdateView());
        }

        private void DestroyAllButtons()
        {
            _buttonsControllers.ForEach(button => UnityEngine.Object.Destroy(button.ButtonView.gameObject));
            _buttonsControllers.Clear();
        }

        private void RecreateButtons()
        {
            DestroyAllButtons();
            SpawnSettingsEmotionButtonsAsync();
        }

        private async void SpawnSettingsEmotionButtonsAsync()
        {
            foreach (var animationConfig in _emotionsAnimationsConfig.EmotionsAnimationConfigs)
            {
                var viewConfig =
                    _emotionsViewConfig.EmotionsConfigs.FirstOrDefault(config =>
                        config.EmotionIndex == animationConfig.EmotionIndex);

                if (viewConfig == null)
                    continue;

                var buttonController = await _settingEmotionsUiFactory.GetNewSetEmotionButtonControllerAsync(
                    _buttonsParent, viewConfig, _settingButtonsScaleMultiplierOnBecomeTarget, _settingButtonsSpeedChangingScaleOnSetTargetState);

                _buttonsControllers.Add(buttonController);

                buttonController.ReturnLastSavedValuesAndUpdateView();

                buttonController.ChangedSelectedMode += OnButtonChangeSelectedMode;

                buttonController.ButtonView.SettingButton.onClick.AddListener(() =>
                    buttonController.TrySettingSelectedModeAndUpdateView(SettingEmotionsModel
                        .GetFirstTemporarySelectedEmotionDataWithoutEmotion));

                buttonController.GotTooMuchEmotionsError += async () =>
                {
                    _errorTextController.SetTextAndStartAnimationAsync(
                        await LocalizationTools.GetLocalizedStringAsync(LocalizationTablesHolder.SettingEmotionsPopup, LocalizationKeysHolder.MaximumIsNEmotions, 
                            SettingEmotionsModel.TemporarySelectedEmotionsData.Count.ToString()));
                };
            }
        }

        private void OnButtonChangeSelectedMode(int? indexInsideCircleBeforeChange, int? newIndexInsideCircle,
            string emotionIndex)
        {
            TryUpdatingSelectedEmotionsInsideCircleIndexes(indexInsideCircleBeforeChange, newIndexInsideCircle,
                emotionIndex);

            ChangedButtonSelectedMode?.Invoke();
        }

        private void TryUpdatingSelectedEmotionsInsideCircleIndexes(int? indexInsideCircleBeforeChange,
            int? newIndexInsideCircle, string emotionIndex)
        {
            if (!indexInsideCircleBeforeChange.HasValue && !newIndexInsideCircle.HasValue)
                return;

            SettingEmotionsModel.UpdateSelectedEmotionData(indexInsideCircleBeforeChange ?? newIndexInsideCircle.Value,
                emotionIndex, newIndexInsideCircle.HasValue);
        }
    }
}