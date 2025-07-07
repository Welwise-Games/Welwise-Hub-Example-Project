using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem
{
    public class ChangesWarningPopup : MonoBehaviour
    {
        [field: SerializeField] public Popup Popup { get; private set; }
        [field: SerializeField] public Button ApplyButton { get; private set; }
        [field: SerializeField] public Button CancelButton { get; private set; }
        [field: SerializeField] public TextMeshProUGUI DescriptionText { get; private set; }
        [field: SerializeField] public TextMeshProUGUI HeaderText { get; private set; }
    }
}