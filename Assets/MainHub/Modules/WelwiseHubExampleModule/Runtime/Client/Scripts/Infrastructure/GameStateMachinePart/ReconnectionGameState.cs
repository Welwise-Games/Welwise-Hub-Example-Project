using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseCharacterModule.Runtime.Client.Scripts.InputServices;
using WelwiseCharacterModule.Runtime.Client.Scripts.MobileHud;
using WelwiseChatModule.Runtime.Client.Scripts.UI;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Circle;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.HubSystem;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.UISystem;
using WelwiseHubExampleModule.Runtime.Client.Scripts.UI;
using WelwiseSharedModule.Runtime.Client.Scripts.Tools;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure.GameStateMachinePart
{
    public class ReconnectionGameState : IGameState
    {
        
        private readonly ShopUIFactory _shopUIFactory;
        private readonly HubFactory _hubFactory;
        private readonly EmotionsCircleFactory _emotionsCircleFactory;
        private readonly ChatFactory _chatFactory;
        private readonly LoadingUIFactory _loadingUIFactory;
        private readonly PlayersFactory _playersFactory;
        private readonly IInputService _inputService;
        private readonly UIFactory _uiFactory;
        private readonly MobileHudFactory _mobileHudFactory;

        public ReconnectionGameState(ShopUIFactory shopUIFactory, HubFactory hubFactory,
            EmotionsCircleFactory emotionsCircleFactory, ChatFactory chatFactory, LoadingUIFactory loadingUIFactory,
            PlayersFactory playersFactory, IInputService inputService, UIFactory uiFactory, MobileHudFactory mobileHudFactory)
        {
            _shopUIFactory = shopUIFactory;
            _hubFactory = hubFactory;
            _emotionsCircleFactory = emotionsCircleFactory;
            _chatFactory = chatFactory;
            _loadingUIFactory = loadingUIFactory;
            _playersFactory = playersFactory;
            _inputService = inputService;
            _uiFactory = uiFactory;
            _mobileHudFactory = mobileHudFactory;
        }

        public async UniTask EnterAsync()
        {
            if (Application.isEditor && !Application.isPlaying)
                return;

            CursorSwitcherTools.TryEnablingCursor();
            await _mobileHudFactory.DisposeUIAsync();
            await _shopUIFactory.DisposeUIAsync();
            await _emotionsCircleFactory.DisposeUIAsync();
            await _chatFactory.DisposeUIAsync();
            await _hubFactory.DisposeAsync();
            await _uiFactory.DisposeUIAsync();
            _playersFactory.DisposePlayers();

            if (_inputService is MobileInputService mobileInputService)
                mobileInputService.ClearMobileHudController();
            
            var loadingGamePopupController = await _loadingUIFactory.GetLoadingGamePopupControllerAsync();
            loadingGamePopupController.Popup.LoadingSlider.value = 0;
            
            loadingGamePopupController.TryEnablingReconnectButton();
            loadingGamePopupController.Popup.Popup.TryOpening();
        }

        public async UniTask ExitAsync()
        {
        }
    }
}