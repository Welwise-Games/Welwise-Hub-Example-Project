using System.Runtime.InteropServices;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure.GameStateMachinePart;
using WelwiseHubExampleModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.EventBusSystem;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.UISystem
{
    public class LoadingGamePopupController
    {
        public readonly LoadingGamePopup Popup;

        public LoadingGamePopupController(LoadingGamePopup popup, EventBus eventBus)
        {
            Popup = popup;
            popup.Popup.TryOpening();
            popup.ReconnectButton.onClick.AddListener(() =>
                eventBus.Fire(new EnterClientStateEvent(GameState.Initialization)));
            popup.ReconnectButton.gameObject.SetActive(false);

#if !UNITY_EDITOR
            HideLoadingAndThreeCanvas();
#endif
        }

        public void TryEnablingReconnectButton()
        {
            if (Popup)
                Popup.ReconnectButton.gameObject.SetActive(true);
        }

        [DllImport("__Internal")]
        private static extern void HideLoadingAndThreeCanvas();
    }
}