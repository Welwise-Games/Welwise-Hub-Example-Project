using UnityEngine;
using UnityEngine.UI;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseUICircleItemModule.Runtime.Scripts
{
    public class CircleButton : MonoBehaviour
    {
        [field: SerializeField] public Button Button { get; private set; }
        [field: SerializeField] public Image RaycastableImage { get; private set; }
        [field: SerializeField] public Image CirclePartImage { get; private set; }
        [field: SerializeField] public Image ItemImage { get; private set; }
        [field: SerializeField] public PointerEnterExitObserver PointerEnterExitObserver { get; private set; }
    }
}