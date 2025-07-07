using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem.SettingEmotions
{
    public class SettingEmotionsPopup : MonoBehaviour
    {
        [field: SerializeField] public Popup Popup { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TooMuchSelectedEmotionsText { get; private set; }
        [field: SerializeField] public Button ClearSelectedEmotions { get; private set; }
    }
}