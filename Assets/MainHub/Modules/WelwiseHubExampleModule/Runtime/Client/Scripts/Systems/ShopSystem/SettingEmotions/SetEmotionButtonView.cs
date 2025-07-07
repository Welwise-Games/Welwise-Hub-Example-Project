using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem.SettingEmotions
{
    public class SetEmotionButtonView : MonoBehaviour
    {
        [field: SerializeField] public Image EmotionNumberBackgroundImage { get; private set; }
        [field: SerializeField] public TextMeshProUGUI EmotionNumberText { get; private set; }
        [field: SerializeField] public TextMeshProUGUI EmotionNameText { get; private set; }
        [field: SerializeField] public Image EmotionNameTextBackgroundImage { get; private set; }
        [field: SerializeField] public Image EmotionViewImage { get; private set; }
        [field: SerializeField] public Button SettingButton { get; private set; }
        [field: SerializeField] public PointerEnterExitObserver PointerEnterExitObserver { get; private set; }
    }
}