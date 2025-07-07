using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem
{
    public class SelectionItemButtonView : MonoBehaviour
    {
        [field: SerializeField] public Button Button { get; private set; }
        [field: SerializeField] public TextMeshProUGUI ItemNameText { get; private set; }
        [field: SerializeField] public Image ItemImage { get; private set; }
        [field: SerializeField] public Image ItemNameTextBackgroundImage { get; private set; }
        [field: SerializeField] public Image SelectedImage { get; private set; }
        [field: SerializeField] public PointerEnterExitObserver PointerEnterExitObserver { get; private set; }
    }
}