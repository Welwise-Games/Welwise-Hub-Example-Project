using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;

namespace WelwiseUICircleItemModule.Runtime.Scripts
{
    public class ItemsCircleWindow : MonoBehaviour
    {
        [field: SerializeField] public MonoBehaviourObserver MonoBehaviourObserver { get; private set; }
        [field: SerializeField] public Popup Popup { get; private set; }
        [field: SerializeField] public Image TargetItemPointerImage { get; private set; }
        [field: SerializeField] public TextMeshProUGUI OpenCircleKeyCodeText { get; private set; }
        [field: SerializeField] public RectTransform OpenCircleKeyCodeParent { get; private set; }
        [field: SerializeField] public Button SetOpenStateButton { get; private set; }
        [field: SerializeField] public CircleButton[] CircleButtons { get; private set; }
        [field: SerializeField] public float CirclePartAlphaHitTestMinimumThreshold { get; private set; } = 0.5f;
        [field: SerializeField] public Color SelectedCircleButtonColor { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TargetItemNameText { get; private set; }
    }
}
