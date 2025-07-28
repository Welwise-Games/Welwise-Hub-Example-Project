using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;
using WelwiseUICircleItemModule.Runtime.Scripts;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts.Circle.CircleWindow
{
    public class EmotionsCircleWindow : MonoBehaviour
    {
        [field: SerializeField] public ItemsCircleWindow ItemsCircleWindow { get; private set; }
    }
}