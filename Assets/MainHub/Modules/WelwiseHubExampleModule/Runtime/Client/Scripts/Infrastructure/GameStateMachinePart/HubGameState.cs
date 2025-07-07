using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseCharacterModule.Runtime.Client.Scripts.InputServices;
using WelwiseChatModule.Runtime.Client.Scripts.UI;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Circle;
using WelwiseGamesSDK.Shared;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.HubSystem;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem.SettingEmotions;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.TrainingSystem;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.UISystem;
using WelwiseHubExampleModule.Runtime.Client.Scripts.UI;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data;
using WelwiseSharedModule.Runtime.Client.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts.NetworkModule;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure.GameStateMachinePart
{
    public class  HubGameState : IGameState
    {
        private readonly ShopUIFactory _shopUIFactory;
        private readonly UIFactory _uiFactory;
        private readonly ChatFactory _chatFactory;
        private readonly PlayersFactory _playersFactory;
        private readonly EmotionsCircleFactory _emotionsCircleFactory;
        private readonly SettingEmotionsUIFactory _settingEmotionsUiFactory;
        private readonly LoadingUIFactory _loadingUIFactory;
        private readonly HubFactory _hubFactory;
        private readonly ISDK _sdk;
        private readonly EnteredToPortalEventProvider _enteredToPortalEventProvider;
        private readonly TrainingFactory _trainingFactory;
        private readonly ClientsConnectionTrackingServiceForClient _clientsConnectionTrackingService;
        private readonly IInputService _inputService;
        private readonly IAssetLoader _assetLoader;
        
        private const string SkyboxAssetId = 
#if ADDRESSABLES
        "Skybox";
#else
        "WelwiseHubExampleModule/Runtime/Client/ZerinLabs_shaderPack_CartoonSky/Materials/Skybox";
#endif
        public HubGameState(ShopUIFactory shopUIFactory, ChatFactory chatFactory, PlayersFactory playersFactory,
            EmotionsCircleFactory emotionsCircleFactory, SettingEmotionsUIFactory settingEmotionsUiFactory,
            LoadingUIFactory loadingUIFactory, 
            HubFactory hubFactory, ISDK sdk, EnteredToPortalEventProvider enteredToPortalEventProvider,
            TrainingFactory trainingFactory, ClientsConnectionTrackingServiceForClient clientsConnectionTrackingService, UIFactory uiFactory, IAssetLoader assetLoader)
        {
            _shopUIFactory = shopUIFactory;
            _chatFactory = chatFactory;
            _playersFactory = playersFactory;
            _emotionsCircleFactory = emotionsCircleFactory;
            _settingEmotionsUiFactory = settingEmotionsUiFactory;
            _loadingUIFactory = loadingUIFactory;
            _hubFactory = hubFactory;
            _sdk = sdk;
            _enteredToPortalEventProvider = enteredToPortalEventProvider;
            _trainingFactory = trainingFactory;
            _clientsConnectionTrackingService = clientsConnectionTrackingService;
            _uiFactory = uiFactory;
            _assetLoader = assetLoader;
        }

        public async UniTask EnterAsync()
        {
            var loadingGamePopupController = await _loadingUIFactory.GetLoadingGamePopupControllerAsync();
            
            var uiRoot = await _uiFactory.GetUIRootAsync();
            
            loadingGamePopupController.Popup.LoadingSlider.value = 0.25f;

            var shopPopupController = _shopUIFactory.GetShopPopupController();

            var chatWindowController = await _chatFactory.GetChatWindowControllerAsync(
                uiRoot.SerializableComponents.transform,
                () => !shopPopupController.ShopPopup.Popup.IsOpen);

            loadingGamePopupController.Popup.LoadingSlider.value = 0.5f;

            await AsyncTools.WaitWhileWithoutSkippingFrame(() => _playersFactory.OwnerPlayerComponents == null);
            
            _playersFactory.OwnerPlayerComponents.CharacterComponents.CursorController?.AddCanSwitchCursorFunc(() =>
                !shopPopupController.ShopPopup.Popup.IsOpen);
            
            _playersFactory.OwnerPlayerComponents.CharacterComponents.CameraController.AddCanSwitchCameraModeFunc(
                () => !chatWindowController.ChatWindow.InputField.isFocused);

            loadingGamePopupController.Popup.LoadingSlider.value = 0.75f;

            var emotionsCircleWindowController = await _emotionsCircleFactory
                .GetEmotionsCircleWindowControllerAsync(
                    uiRoot.SerializableComponents.transform,
                    () => !chatWindowController.ChatWindow.InputField.isFocused,
                    () => !shopPopupController.ShopPopup.Popup.IsOpen,
                    _playersFactory.OwnerPlayerComponents.ClientComponents.PlayerEmotionsComponents);

            loadingGamePopupController.Popup.LoadingSlider.value = 0.9f;

            await CreateAndSubscribeSettingEmotionsPopupAsync();

            loadingGamePopupController.Popup.LoadingSlider.value = 1f;

            RenderSettings.skybox = await AssetProvider.LoadAsync<Material>(SkyboxAssetId, 
                _assetLoader);

            await TrainingTools.InitializeTrainingProcessAsync(_sdk.PlayerData, shopPopupController,
                _playersFactory.OwnerPlayerComponents.SerializableComponents.transform,
                _hubFactory.ClientHubComponents.SerializableComponents.ShopSerializableComponents
                    .PositionProviderTransform, _sdk.PlatformNavigation,
                _enteredToPortalEventProvider, _trainingFactory, uiRoot.SerializableComponents.transform,
                _clientsConnectionTrackingService);

            loadingGamePopupController.Popup.Popup.TryClosing();
        }

        public async UniTask ExitAsync()
        {
        }

        private async UniTask CreateAndSubscribeSettingEmotionsPopupAsync()
        {
            var shopPopupController = _shopUIFactory.GetShopPopupController();

            var settingEmotionsPopupController = await
                _settingEmotionsUiFactory.GetSettingEmotionsPopupControllerAsync(
                    shopPopupController.ShopPopup.ItemsParentSafeAreaTransform,
                    shopPopupController.ShopPopup.SelectionItemButtonsParent,
                    shopPopupController.ShopPopup.SelectionItemButtonTargetStateAnimationConfig
                        .ScaleMultiplierOnBecomeTarget,
                    shopPopupController.ShopPopup.SelectionItemButtonTargetStateAnimationConfig
                        .SpeedChangingScaleOnSetTargetState);

            settingEmotionsPopupController.SettingPopup.Popup.TryClosing();

            shopPopupController.SelectedItemCategory += category =>
            {
                settingEmotionsPopupController.SettingPopup.Popup.TrySettingOpenState(
                    category is ItemCategory.Emotions or ItemCategory.All);

                if (category == ItemCategory.Emotions)
                    settingEmotionsPopupController.InitializeOnOpen();
                else if (category != ItemCategory.All)
                    settingEmotionsPopupController.DeInitializeOnClose();
            };

            shopPopupController.CreatedAllButtons += category =>
            {
                if (category == ItemCategory.All)
                    settingEmotionsPopupController.InitializeOnOpen();
            };

            shopPopupController.ShopSettingEquippedItemsModel.RevertedChanges +=
                settingEmotionsPopupController.ReturnLastSavedValuesAndUpdateView;
            shopPopupController.ShopSettingEquippedItemsModel.AppliedChanges +=
                settingEmotionsPopupController.SettingEmotionsModel.ApplyChanges;

            shopPopupController.ShopPopup.Popup.Closed += settingEmotionsPopupController.SettingPopup.Popup.TryClosing;
            shopPopupController.ShopSettingEquippedItemsModel.AddModifiable(settingEmotionsPopupController
                .SettingEmotionsModel);

            shopPopupController.ShopPopup.transform.SetAsLastSibling();
        }
    }
}