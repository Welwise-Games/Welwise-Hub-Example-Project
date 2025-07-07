using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem
{
    public class ChangingNicknamePopup : MonoBehaviour
    {
        [field: SerializeField] public Popup Popup { get; private set; }
        [field: SerializeField] public Button OpenPopupButton { get; private set; }
        [field: SerializeField] public Button ClosePopupButton { get; private set; }
        [field: SerializeField] public Button ApplyButton { get; private set; }
        [field: SerializeField] public Button CancelButton { get; private set; }
        [field: SerializeField] public TMP_InputField PlayerNicknameInputField { get; private set; }
        [field: SerializeField] public TextMeshProUGUI OnlyLatinLettersText { get; private set; }
    }
}