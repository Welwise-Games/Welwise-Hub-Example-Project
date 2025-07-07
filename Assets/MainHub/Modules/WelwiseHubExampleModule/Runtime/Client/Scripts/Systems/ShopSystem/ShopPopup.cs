using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem
{
    public class ShopPopup : MonoBehaviour
    {
        [field: SerializeField] public SelectionItemButtonTargetStateAnimationConfig SelectionItemButtonTargetStateAnimationConfig { get; private set; }
        [field: SerializeField] public PointerDragObserver PlayerViewRawImagePointerDragObserver { get; private set; }
        [field: SerializeField] public PointerUpDownObserver PlayerViewRawImagePointerUpDownObserver { get; private set; }
        [field: SerializeField] public Transform SelectionItemButtonsParent { get; private set; }
        [field: SerializeField] public Transform ItemsParentSafeAreaTransform { get; private set; }
        [field: SerializeField] public Popup Popup { get; private set; }
        [field: SerializeField] public ChangesWarningPopup ChangesWarningPopup { get; private set; }
        [field: SerializeField] public ChangingNicknamePopup ChangingNicknamePopup { get; private set; }
        [field: SerializeField] public ChangingItemsColorPopup ChangingItemsColorPopup { get; private set; }
        [field: SerializeField] public ChangingSelectionItemsPopup ChangingSelectionItemsPopup { get; private set; }
        [field: SerializeField] public Button ApplyChangesButton { get; private set; }
        [field: SerializeField] public Button CloseShopPopupButton { get; private set; }
        [field: SerializeField] public Button RevertChangesButton { get; private set; }
        [field: SerializeField] public TextMeshProUGUI PlayerNicknameText { get; private set; }
        [field: SerializeField] public Transform SelectionTargetItemCategoryImageParent { get; private set; }
        [field: SerializeField] [field: Range(1, 5)] public float SelectionTargetItemCategoryImageAnimationDuration { get; private set; } = 1;
        [field: SerializeField] public SelectionItemCategoryButton[] SelectionItemCategoryButtons { get; private set; }
        [field: SerializeField] public RectTransform SelectionItemCategoryButtonsParent { get; private set; }
    }
}