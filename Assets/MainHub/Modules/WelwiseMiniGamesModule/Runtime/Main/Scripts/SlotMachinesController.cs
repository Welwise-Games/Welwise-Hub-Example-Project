using System;
using System.Linq;
using UnityEngine;
using WelwiseCurrenciesModule.Runtime.Shared.Scripts;
using WelwiseMiniGamesModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace Modules.WelwiseMiniGamesModule.Runtime.Main.Scripts
{
    public class SlotMachinesController
    {
        public MiniGameSerializableComponents CurrentMiniGameInstance { get; private set; }
        public event Action StartedMiniGame, EndedMiniGame;


        private readonly MiniGamesFactory _miniGamesFactory;
        private readonly MiniGamesConfigProviderService _miniGamesConfigProviderService;
        private readonly CurrenciesProviderService _currenciesProviderService;
        private readonly Transform _popupParent;

        public SlotMachinesController(SlotMachineView[] machines, MiniGamesFactory miniGamesFactory,
            MiniGamesConfigProviderService miniGamesConfigProviderService, CurrenciesProviderService currenciesProviderService, Transform popupParent)
        {
            _miniGamesFactory = miniGamesFactory;
            _miniGamesConfigProviderService = miniGamesConfigProviderService;
            _currenciesProviderService = currenciesProviderService;
            _popupParent = popupParent;
            machines.ForEach(InitializeMachine);
        }

        private void InitializeMachine(SlotMachineView slotMachineView)
        {
            var slotMachineController = new SlotMachineController(slotMachineView);

            slotMachineController.StartedMiniGame += TryStartingMiniGameAsync;
        }

        private async void TryStartingMiniGameAsync(MiniGame miniGame)
        {
            var miniGamesConfig = await _miniGamesConfigProviderService.GetMiniGamesConfigAsync();
            
            var config =
                miniGamesConfig.MiniGamesConfigs.FirstOrDefault(config =>
                    config.MiniGame == miniGame);

            if (config == null)
                return;

            var popupInstance = await _miniGamesFactory.GetMiniGamesPopupViewAsync(_popupParent);
            var miniGameInstance = await _miniGamesFactory.GetMiniGameSerializableComponentsInstance(config);

            CurrentMiniGameInstance = miniGameInstance;

            miniGameInstance.GotRewardProvider.Got += AddReward;
            
            popupInstance.Popup.TryOpening();
            
            popupInstance.CloseButton.onClick.AddListener(StopMiniGameAsync);
            
            void AddReward()
            {
                _currenciesProviderService.CurrencyByType[CurrencyType.Soft].TrySettingNumber(
                    _currenciesProviderService.CurrencyByType[CurrencyType.Soft].Number +
                    config.Reward, SetCurrencyNumberReason.MiniGameReward);
            }

            StartedMiniGame?.Invoke();
        }

        private async void StopMiniGameAsync()
        {
            var popupInstance = await _miniGamesFactory.GetMiniGamesPopupViewAsync(_popupParent);
            popupInstance.CloseButton.onClick.RemoveAllListeners();
            popupInstance.Popup.TryClosing();

            if (CurrentMiniGameInstance)
            {
                CurrentMiniGameInstance.GotRewardProvider.ClearEvent();
                UnityEngine.Object.Destroy(CurrentMiniGameInstance.gameObject);   
            }
            
            EndedMiniGame?.Invoke();
        }
    }
}