using UnityEngine;
using UnityEngine.UI;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem
{
    public class SelectionMaterialColorButtonView : MonoBehaviour
    {
        [field: SerializeField] public Image SelectedImage { get; private set; }
        [field: SerializeField] public Button Button { get; private set; }
        [field: SerializeField] public Image ColorImage { get; private set; }
    }
}