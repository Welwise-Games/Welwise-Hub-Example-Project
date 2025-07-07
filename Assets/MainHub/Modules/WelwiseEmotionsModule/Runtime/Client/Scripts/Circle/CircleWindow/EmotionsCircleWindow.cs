using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts.Circle.CircleWindow
{
    public class EmotionsCircleWindow : MonoBehaviour
    {
        [field: SerializeField] public MonoBehaviourObserver MonoBehaviourObserver { get; private set; }
        [field: SerializeField] public Popup Popup { get; private set; }
        [field: SerializeField] public Image TargetEmotionPointerImage { get; private set; }
        [field: SerializeField] public TextMeshProUGUI OpenCircleKeyCodeText { get; private set; }
        [field: SerializeField] public RectTransform OpenCircleKeyCodeParent { get; private set; }
        [field: SerializeField] public Button SetOpenStateButton { get; private set; }
        [field: SerializeField] public PlayingEmotionButton[] PlayEmotionButtons { get; private set; }
        [field: SerializeField] public float CirclePartAlphaHitTestMinimumThreshold { get; private set; } = 0.5f;
        [field: SerializeField] public Color SelectedPlayEmotionButtonsColor { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TargetEmotionNameText { get; private set; }
    }
}