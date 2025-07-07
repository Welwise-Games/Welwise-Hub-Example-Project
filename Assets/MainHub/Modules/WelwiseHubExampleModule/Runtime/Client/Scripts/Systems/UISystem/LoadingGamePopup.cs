using UnityEngine;
using UnityEngine.UI;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.UISystem
{ 
    public class LoadingGamePopup : MonoBehaviour
    {
        [field: SerializeField] public Button ReconnectButton { get; private set; }
        [field: SerializeField] public Popup Popup { get; private set; }
        [field: SerializeField] public Slider LoadingSlider { get; private set; }
    }
}