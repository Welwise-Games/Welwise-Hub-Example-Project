using System;
using DG.Tweening;
using UnityEngine;
using WelwiseEmotionsModule.Runtime.Client.Scripts;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations.Network.Owner;
using WelwiseEmotionsModule.Runtime.Shared.Scripts.Animations.Network;
using WelwiseSharedModule.Runtime.Client.Scripts.Localization;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem.SettingEmotions
{
    public class SetEmotionButtonController
    {
        private bool IsSelected => TargetEmotionInsideCircleIndex.HasValue;
        public int? TargetEmotionInsideCircleIndex { get; private set; }

        public event Action GotTooMuchEmotionsError;
        public event Action<int?, int?, string> ChangedSelectedMode;

        private int? _lastSavedTargetEmotionInsideCircleIndex;
        public readonly SetEmotionButtonView ButtonView;

        private readonly EmotionViewConfig _targetEmotionViewConfig;
        private readonly OwnerSelectedEmotionsDataProviderService _ownerSelectedEmotionsDataProvider;
        private readonly float _scaleMultiplierOnBecomeTarget;
        private readonly float _speedChangingScaleOnSetTargetState;

        public SetEmotionButtonController(SetEmotionButtonView buttonView, EmotionViewConfig targetEmotionViewConfig,
            OwnerSelectedEmotionsDataProviderService ownerSelectedEmotionsDataProvider,
            float scaleMultiplierOnBecomeTarget, float speedChangingScaleOnSetTargetState)
        {
            ButtonView = buttonView;
            _targetEmotionViewConfig = targetEmotionViewConfig;
            _ownerSelectedEmotionsDataProvider = ownerSelectedEmotionsDataProvider;
            _scaleMultiplierOnBecomeTarget = scaleMultiplierOnBecomeTarget;
            _speedChangingScaleOnSetTargetState = speedChangingScaleOnSetTargetState;

            buttonView.EmotionViewImage.sprite = targetEmotionViewConfig.Sprite;

            ChangedSelectedMode += (_, _, _) => UpdateViewAsync();
            
            UpdateTargetStateView(false);
            
            ButtonView.PointerEnterExitObserver.EnteredWithoutArgs += () => UpdateTargetStateView(true);
            ButtonView.PointerEnterExitObserver.ExitedWithoutArgs += () => UpdateTargetStateView(false);
        }
        
        private void UpdateTargetStateView(bool isTargetState)
        {
            ButtonView.EmotionNameTextBackgroundImage.gameObject.SetActive(isTargetState);
            
            ButtonView.EmotionViewImage.transform.DOScale(
                isTargetState ? _scaleMultiplierOnBecomeTarget * Vector3.one : Vector3.one,
                _speedChangingScaleOnSetTargetState);
        }

        public void ReturnLastSavedValuesAndUpdateView()
        {
            _lastSavedTargetEmotionInsideCircleIndex = _ownerSelectedEmotionsDataProvider
                .GetEmotionDataByEmotionIndex(_targetEmotionViewConfig.EmotionIndex)
                ?.IndexInsideCircle;
            TargetEmotionInsideCircleIndex = _lastSavedTargetEmotionInsideCircleIndex;
            UpdateViewAsync();
        }

        public void TrySettingUnselectedModeAndUpdateView()
        {
            if (!IsSelected)
                return;

            var indexInsideCircleBeforeChange = TargetEmotionInsideCircleIndex;

            TargetEmotionInsideCircleIndex = null;

            ChangedSelectedMode?.Invoke(indexInsideCircleBeforeChange, TargetEmotionInsideCircleIndex,
                _targetEmotionViewConfig.EmotionIndex);
        }

        public void TrySettingSelectedModeAndUpdateView(
            Func<SelectedEmotionData> getLocalFirstSelectedDataWithoutEmotion)
        {
            var indexInsideCircleBeforeChange = TargetEmotionInsideCircleIndex;

            if (!IsSelected)
            {
                var newData = getLocalFirstSelectedDataWithoutEmotion?.Invoke();

                if (newData == null)
                {
                    GotTooMuchEmotionsError?.Invoke();
                    return;
                }

                TargetEmotionInsideCircleIndex = newData.IndexInsideCircle;
            }
            else
            {
                TargetEmotionInsideCircleIndex = null;
            }


            ChangedSelectedMode?.Invoke(indexInsideCircleBeforeChange, TargetEmotionInsideCircleIndex,
                _targetEmotionViewConfig.EmotionIndex);
        }

        private async void UpdateViewAsync()
        {
            ButtonView.EmotionNumberBackgroundImage.gameObject.SetActive(IsSelected);
            
            if (IsSelected)
                ButtonView.EmotionNumberText.text = (TargetEmotionInsideCircleIndex + 1).ToString();
            
            ButtonView.EmotionNameText.text = await LocalizationTools.GetLocalizedStringAsync(LocalizationTablesHolder.Emotions,
                _targetEmotionViewConfig.EmotionIndex);
        }
    }
}