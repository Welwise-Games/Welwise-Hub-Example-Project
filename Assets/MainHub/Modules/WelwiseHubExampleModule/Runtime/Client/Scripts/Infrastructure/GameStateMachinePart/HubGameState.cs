using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseCharacterModule.Runtime.Client.Scripts.InputServices;
using WelwiseChatModule.Runtime.Client.Scripts.UI;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Circle;
using WelwiseGamesSDK.Shared;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.HubSystem;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem.SetEmotions;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.TrainingSystem;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.UISystem;
using WelwiseHubExampleModule.Runtime.Client.Scripts.UI;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data;
using WelwiseItemInShopModule.Client.Scripts;
using WelwiseItemInShopModule.Shared.Scripts;
using WelwisePetsModule.Runtime.Client.Scripts.SetPet;
using WelwiseSharedModule.Runtime.Client.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts.NetworkModule;
using WelwiseSharedModule.Runtime.Client.Scripts.Tools;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure.GameStateMachinePart
{
    public class HubGameState : IGameState
    {
        private readonly ShopUIFactory _shopUIFactory;
        private readonly UIFactory _uiFactory;
        private readonly ChatFactory _chatFactory;
        private readonly PlayersFactory _playersFactory;
        private readonly EmotionsCircleFactory _emotionsCircleFactory;
        private readonly SetEmotionsUIFactory _setEmotionsUIFactory;
        private readonly SetPetsUIFactory _setPetsUIFactory;
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
            EmotionsCircleFactory emotionsCircleFactory, SetEmotionsUIFactory setEmotionsUIFactory,
            LoadingUIFactory loadingUIFactory,
            HubFactory hubFactory, ISDK sdk, EnteredToPortalEventProvider enteredToPortalEventProvider,
            TrainingFactory trainingFactory, ClientsConnectionTrackingServiceForClient clientsConnectionTrackingService,
            UIFactory uiFactory, IAssetLoader assetLoader, SetPetsUIFactory setPetsUIFactory)
        {
            _shopUIFactory = shopUIFactory;
            _chatFactory = chatFactory;
            _playersFactory = playersFactory;
            _emotionsCircleFactory = emotionsCircleFactory;
            _setEmotionsUIFactory = setEmotionsUIFactory;
            _loadingUIFactory = loadingUIFactory;
            _hubFactory = hubFactory;
            _sdk = sdk;
            _enteredToPortalEventProvider = enteredToPortalEventProvider;
            _trainingFactory = trainingFactory;
            _clientsConnectionTrackingService = clientsConnectionTrackingService;
            _uiFactory = uiFactory;
            _assetLoader = assetLoader;
            _setPetsUIFactory = setPetsUIFactory;
        }

        public async UniTask EnterAsync()
        {
            CursorSwitchTools.TryDisablingCursor();

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

            _playersFactory.OwnerPlayerComponents.CharacterComponents.CameraController.AddCanSwitchCameraModeFunc(() =>
                !chatWindowController.ChatWindow.InputField.isFocused);

            loadingGamePopupController.Popup.LoadingSlider.value = 0.75f;

            var emotionsCircleWindowController = await _emotionsCircleFactory
                .GetEmotionsCircleWindowControllerAsync(
                    uiRoot.SerializableComponents.transform,
                    () => !chatWindowController.ChatWindow.InputField.isFocused,
                    () => !shopPopupController.ShopPopup.Popup.IsOpen,
                    _playersFactory.OwnerPlayerComponents.ClientComponents.PlayerEmotionsComponents);

            loadingGamePopupController.Popup.LoadingSlider.value = 0.9f;

            await CreateAndSubscribeSetEmotionsPopupAndPetsPopupAsync();

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

        public UniTask ExitAsync() => UniTask.CompletedTask;

        private async UniTask CreateAndSubscribeSetEmotionsPopupAndPetsPopupAsync()
        {
            var shopPopupController = _shopUIFactory.GetShopPopupController();

            var setEmotionsPopupController = await
                _setEmotionsUIFactory.GetSetEmotionsPopupControllerAsync(
                    shopPopupController.ShopPopup.ItemsParentSafeAreaTransform,
                    shopPopupController.ShopPopup.SelectionItemButtonsParent,
                    shopPopupController.ShopPopup.SelectionItemButtonTargetStateAnimationConfig
                        .ScaleMultiplierOnBecomeTarget,
                    shopPopupController.ShopPopup.SelectionItemButtonTargetStateAnimationConfig
                        .SpeedChangingScaleOnSetTargetState);

            var setPetsPopupController = await
                _setPetsUIFactory.GetSetPetsPopupControllerAsync(
                    shopPopupController.ShopPopup.ItemsParentSafeAreaTransform,
                    shopPopupController.ShopPopup.SelectionItemButtonsParent,
                    shopPopupController.ShopPopup.SelectionItemButtonTargetStateAnimationConfig
                        .ScaleMultiplierOnBecomeTarget,
                    shopPopupController.ShopPopup.SelectionItemButtonTargetStateAnimationConfig
                        .SpeedChangingScaleOnSetTargetState);

            setPetsPopupController.SetItemsModel.UpdatedTemporaryData += data =>
                _hubFactory.ClientHubComponents.ShopController.PlayerPetsViewController.UpdatePetsViewAsync(data)
                    .Forget();

            SubscribeSetItemPopupController(setEmotionsPopupController, shopPopupController, ItemCategory.Emotions);
            SubscribeSetItemPopupController(setPetsPopupController, shopPopupController, ItemCategory.Pets);

            shopPopupController.ShopPopup.transform.SetAsLastSibling();
        }

        private void SubscribeSetItemPopupController<T1, T2>(SetItemsPopupController<T1, T2> setItemPopupController,
            ShopPopupController shopPopupController, ItemCategory targetCategory) where T1 : class, ISelectedItemData
            where T2 : IClientSelectedItemsData<T1>
        {
            setItemPopupController.SetItemsPopup.Popup.TryClosing();

            shopPopupController.SelectedItemCategory += category =>
            {
                setItemPopupController.SetItemsPopup.Popup.TrySettingOpenState(
                    category is ItemCategory.Emotions or ItemCategory.All);

                if (category == targetCategory)
                    setItemPopupController.InitializeOnOpen(true);
                else if (category != ItemCategory.All)
                {
                    setItemPopupController.DeInitializeOnClose();
                }
            };

            shopPopupController.CreatedAllButtons += category =>
            {
                if (category == ItemCategory.All)
                    setItemPopupController.InitializeOnOpen(false);
            };

            shopPopupController.ShopSetEquippedItemsModel.RevertedChanges +=
                setItemPopupController.ReturnLastSavedValuesAndUpdateView;
            shopPopupController.ShopSetEquippedItemsModel.AppliedChanges +=
                setItemPopupController.SetItemsModel.ApplyChanges;

            shopPopupController.ShopPopup.Popup.Closed += setItemPopupController.SetItemsPopup.Popup.TryClosing;
            shopPopupController.ShopSetEquippedItemsModel.AddModifiable(setItemPopupController
                .SetItemsModel);
        }
    }
}