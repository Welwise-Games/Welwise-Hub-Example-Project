using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseItemInShopModule.Client.Scripts
{
    public class SetItemButtonView : MonoBehaviour
    {
        [field: SerializeField] public Image ItemOrdinalIndexBackgroundImage { get; private set; }
        [field: SerializeField] public TextMeshProUGUI ItemOrdinalIndexText { get; private set; }
        [field: SerializeField] public TextMeshProUGUI ItemNameText { get; private set; }
        [field: SerializeField] public Image ItemNameTextBackgroundImage { get; private set; }
        [field: SerializeField] public Image ItemViewImage { get; private set; }
        [field: SerializeField] public Button SetButton { get; private set; }
        [field: SerializeField] public PointerEnterExitObserver PointerEnterExitObserver { get; private set; }
    }
}