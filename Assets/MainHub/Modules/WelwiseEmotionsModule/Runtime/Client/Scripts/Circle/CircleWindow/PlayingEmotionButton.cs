using UnityEngine;
using UnityEngine.UI;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts.Circle.CircleWindow
{
    public class PlayingEmotionButton : MonoBehaviour
    {
        [field: SerializeField] public Button Button { get; private set; }
        [field: SerializeField] public Image RaycastableImage { get; private set; }
        [field: SerializeField] public Image CirclePartImage { get; private set; }
        [field: SerializeField] public Image EmotionImage { get; private set; }
        [field: SerializeField] public PointerEnterExitObserver PointerEnterExitObserver { get; private set; }
    }
}