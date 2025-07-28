using UnityEngine;
using UnityEngine.UI;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace Modules.WelwiseMiniGamesModule.Runtime.Main.Scripts
{
    public class MiniGamesPopupView : MonoBehaviour
    {
        [field: SerializeField] public Button CloseButton { get; private set; }
        [field: SerializeField] public Popup Popup { get; private set; }
    }
}