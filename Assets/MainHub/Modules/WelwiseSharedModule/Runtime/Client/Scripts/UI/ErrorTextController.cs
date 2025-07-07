using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
#if WELWISE_SHARED_MODULE_LOCALIZATION
using WelwiseSharedModule.Runtime.Client.Scripts.Localization;
#endif

namespace WelwiseSharedModule.Runtime.Client.Scripts.UI
{
    public class ErrorTextController
    {
        private readonly TextMeshProUGUI _text;
        private readonly ErrorTextConfig _config;
        private TweenerCore<Color, Color, ColorOptions> _currentAnimation;
        private readonly Color _defaultColor;

        public ErrorTextController(TextMeshProUGUI text, ErrorTextConfig config)
        {
            _text = text;
            _config = config;
            _defaultColor = text.color;

            ChangeTextColor(GetDefaultColorWithZeroTransparency());
        }

        public async void SetTextAndStartAnimationAsync(string localizationKey, string tableName = null)
        {
#if WELWISE_SHARED_MODULE_LOCALIZATION
            ChangeText(tableName != null
                ? await LocalizationTools.GetLocalizedStringAsync(tableName, localizationKey)
                : localizationKey);
#else
            ChangeText(localizationKey);
#endif

            Debug.Log($"{localizationKey}");

            StartAnimation();
        }

        private void StartAnimation()
        {
            StopAnimation();
            ChangeTextColor(_defaultColor);
            Animate();
        }

        private TweenerCore<Color, Color, ColorOptions> FadeIn() =>
            _currentAnimation = _text.DOFade(_defaultColor.a, _config.FadeInTime);

        private TweenerCore<Color, Color, ColorOptions> FadeOut() =>
            _currentAnimation = _text.DOFade(0, _config.FadeOutTime);

        private void StopAnimation() => _currentAnimation?.Kill();

        private void ChangeText(string text) => _text.text = text;

        private void ChangeTextColor(Color color) => _text.color = color;

        private Color GetDefaultColorWithZeroTransparency() =>
            new(_defaultColor.r, _defaultColor.g, _defaultColor.b, 0);

        private void Animate()
        {
            ChangeTextColor(GetDefaultColorWithZeroTransparency());
            FadeIn().OnComplete(() => FadeOut());
        }
    }
}