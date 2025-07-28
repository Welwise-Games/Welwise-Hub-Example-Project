using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseItemInShopModule.Client.Scripts
{
    public class SetItemsPopup : MonoBehaviour
    {
        [field: SerializeField] public Popup Popup { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TooMuchSelectedItemsText { get; private set; }
        [field: SerializeField] public Button ClearSelectedItems { get; private set; }
    }
}