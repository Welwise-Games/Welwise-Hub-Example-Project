using UnityEngine;
using UnityEngine.UI;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem
{
    public class ChangingItemsColorPopup : MonoBehaviour
    {
        [field: SerializeField] public Popup Popup { get; private set; }
        [field: SerializeField] public Transform SelectionItemButtonsParent { get; private set; }
        [field: SerializeField] public Button[] SelectionMaterialIndexButtons { get; private set; }
        [field: SerializeField] public Slider ChangingColorSlider { get; private set; }
    }
}