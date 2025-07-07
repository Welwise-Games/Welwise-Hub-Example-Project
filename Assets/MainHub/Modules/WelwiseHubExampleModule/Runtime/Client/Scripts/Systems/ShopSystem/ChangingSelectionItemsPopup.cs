using UnityEngine;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem
{
    public class ChangingSelectionItemsPopup : MonoBehaviour
    {
        [field: SerializeField] public Popup Popup { get; private set; }
        [field: SerializeField] public Transform SelectionItemButtonsParent { get; private set; }
    }
}