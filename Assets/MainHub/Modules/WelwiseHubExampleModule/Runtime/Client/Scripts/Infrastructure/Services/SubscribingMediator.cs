using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FishNet.Connection;
using FishNet.Managing.Client;
using UnityEngine;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;
using WelwiseChangingNicknameModule.Runtime.Client.Scripts;
using WelwiseChangingNicknameModule.Runtime.Client.Scripts.Services;
using WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Network.Broadcasts;
using WelwiseCharacterModule.Runtime.Shared.Scripts;
using WelwiseChatModule.Runtime.Client.Scripts.Network;
using WelwiseChatModule.Runtime.Client.Scripts.UI;
using WelwiseChatModule.Runtime.Client.Scripts.UI.Window;
using WelwiseChatModule.Runtime.Shared.Scripts.Network;
using WelwiseClothesSharedModule.Runtime.Client.Scripts;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseEmotionsModule.Runtime.Client.Scripts;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations.Network.Owner;
using WelwiseEmotionsModule.Runtime.Shared.Scripts;
using WelwiseGamesSDK.Shared;
using WelwiseGamesSDK.Shared.Modules;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure.GameStateMachinePart;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.HubSystem;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.UISystem;
using WelwiseHubExampleModule.Runtime.Shared.Scripts;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Network;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Systems.HubSystem.Network;
using WelwiseLoadingClothesModule.Runtime.Client.Scripts;
using WelwiseNicknameSharedModule.Runtime.Client.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts.NetworkModule;
using WelwiseSharedModule.Runtime.Client.Scripts.Tools;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.EventBusSystem;
using Channel = FishNet.Transporting.Channel;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Infrastructure.Services
{
    public class SubscribingMediator
    {
        private readonly PlayersFactory _playersFactory;
        private readonly HubFactory _hubFactory;
        private readonly EventBus _eventBus;
        private readonly ClientsDataProviderService _clientsDataProviderService;
        private readonly ClientsCustomizationDataProviderService _clientsCustomizationDataProviderService;
        private readonly ClientsNicknamesProviderService _clientsNicknamesProviderService;
        private readonly ClientManager _clientManager;
        private readonly ISDK _sdk;

        private const string PlayerAppearanceDataFieldNameForMetaverseSavings = "PlayerAppearanceData";

        public SubscribingMediator(PlayersFactory playersFactory, HubFactory hubFactory,
            ClientsConnectionTrackingServiceForClient clientsConnectionTrackingServiceForClient,
            OwnerEmotionsPlayingSynchronizerService ownerEmotionsPlayingSynchronizerService, EventBus eventBus,
            ShopUIFactory shopUIFactory, ClientsDataProviderService clientsDataProviderService,
            ClientsCustomizationDataProviderService clientsCustomizationDataProviderService,
            ChatsDataProviderService chatsDataProviderService, ClientManager clientManager, IEnvironment environment,
            ISDK sdk, ClientsNicknamesProviderService clientsNicknamesProviderService, ChatFactory chatFactory)
        {
            _playersFactory = playersFactory;
            _hubFactory = hubFactory;
            _eventBus = eventBus;
            _clientsDataProviderService = clientsDataProviderService;
            _clientsCustomizationDataProviderService = clientsCustomizationDataProviderService;
            _clientManager = clientManager;
            _sdk = sdk;
            _clientsNicknamesProviderService = clientsNicknamesProviderService;

            clientManager.RegisterBroadcast<ClientsServicesInitializationBroadcast>(
                InitializeClientsServices);
            clientManager.RegisterBroadcast<ClientServicesInitializationBroadcast>(
                InitializeClientServices);

            clientManager.RegisterBroadcast<PlayerInitializationDependencies>(InitializePlayer);
            clientManager.RegisterBroadcast<PlayersInitializationBroadcast>(InitializePlayers);

            clientManager.RegisterBroadcast<HubInitializationDependencies>(InitializeHubAndEnterHubState);

            clientManager.RegisterBroadcast<SettingClientCustomizationDataBroadcastForClient>(
                SetPlayerCustomizationData);

            clientManager.OnAuthenticated += clientsConnectionTrackingServiceForClient.InvokeConnectedActionForOwner;

            clientManager.OnClientConnectionState +=
                clientsConnectionTrackingServiceForClient.TryInvokingDisconnectedActionForOwner;

            clientManager.OnRemoteConnectionState +=
                clientsConnectionTrackingServiceForClient.InvokeActionByConnectionStateForNotOwner;

            chatsDataProviderService.AddedMessageData += DisplayMessageOverPlayer;

            chatFactory.CreatedChatWindowController += SubscribeChatWindowAsync;
                
            void SubscribeChatWindowAsync(ChatWindowController chatWindowController)
            {
                chatWindowController.ChatWindow.InputField.onSelect.AddListener(text =>
                {
                    if (_playersFactory.OwnerPlayerComponents != null)
                        _playersFactory.OwnerPlayerComponents.CharacterComponents.MovementController.IsEnabled = false;
                });

                chatWindowController.ChatWindow.InputField.onDeselect.AddListener(text =>
                {
                    if (_playersFactory.OwnerPlayerComponents != null)
                        _playersFactory.OwnerPlayerComponents.CharacterComponents.MovementController.IsEnabled = true;
                });
            }

            playersFactory.OwnerPlayerInitialized += components =>
            {
                components.ClientComponents.PlayerEmotionsComponents.SubscribeAnimatorController(
                    ownerEmotionsPlayingSynchronizerService);
            };

            playersFactory.ClientPlayerInitialized += (networkConnection, components)
                =>
            {
                var playerNicknameTextController = new PlayerNicknameTextController(
                    components.SerializableComponents.NicknameText,
                    clientsNicknamesProviderService.Nicknames[networkConnection]);

                NicknameChangingTools.InitializePlayer(clientsNicknamesProviderService, playerNicknameTextController,
                    networkConnection);
            };

            _clientsDataProviderService.AddedData += (networkConnection, clientData) =>
            {
                if (!networkConnection.IsOwners())
                    return;

                TrySavingOwnerEquippedItemsDataForMetaverse(sdk, networkConnection, clientData.CustomizationData);
            };

            _clientsCustomizationDataProviderService.ChangedClientCustomizationData +=
                (networkConnection, data) => TrySavingOwnerEquippedItemsDataForMetaverse(sdk, networkConnection, data);

            shopUIFactory.CreatedShopPopupController +=
                shopPopupController =>
                {
                    SubscribeShopSettingEquippedItemsModelForChangingOwnerName(shopPopupController
                        .ShopSettingEquippedItemsModel);
                };

            shopUIFactory.CreatedShopPopupController += shopPopupController =>
                SubscribeShopSettingEquippedItemsModelForChangingClientPlayerCustomizationData(shopPopupController
                    .ShopSettingEquippedItemsModel);

            clientsConnectionTrackingServiceForClient.Disconnected += _playersFactory.TryRemovingPlayer;
            clientsConnectionTrackingServiceForClient.Disconnected += TryRemovingServiceFromClientsDataProviderService;
            clientsConnectionTrackingServiceForClient.OwnerDisconnected +=
                () => eventBus.Fire(new EnterClientStateEvent(GameState.Reconnection));

            clientsConnectionTrackingServiceForClient.OwnerConnected +=
                () =>
                {
                    clientManager.Broadcast(new LoginBroadcast(new ClientData(
                        new ClientAccountData(null, sdk.PlayerData.GetPlayerName()),
                        sdk.PlayerData.MetaverseData
                            .GetString(EmotionsEntryPointTools.SelectedEmotionsDataFieldNameForMetaverseSavings)
                            ?.GetDeserializedWithoutNulls<ClientSelectedEmotionsData>(), new CustomizationData(
                            sdk.PlayerData.MetaverseData.GetString(PlayerAppearanceDataFieldNameForMetaverseSavings)
                                ?.GetDeserializedWithoutNulls<ModelAppearanceData>(),
                            sdk.PlayerData.GetEquippedItemsDataFromMetaverse()))));
                };
        }

        private void TrySavingOwnerEquippedItemsDataForMetaverse(ISDK sdk, NetworkConnection networkConnection,
            CustomizationData data)
        {
            SetOwnersMetaverseStringData(sdk, networkConnection,
                ClothesSharedTools.EquippedItemsDataFieldNameForMetaverseSavings,
                () => data.EquippedItemsData.GetJsonSerializedObjectWithoutNulls());

            SetOwnersMetaverseStringData(sdk, networkConnection,
                PlayerAppearanceDataFieldNameForMetaverseSavings,
                () => data.AppearanceData.GetJsonSerializedObjectWithoutNulls());
        }

        private void SetOwnersMetaverseStringData(ISDK sdk, NetworkConnection networkConnection,
            string metaverseFieldName, Func<string> dataFunc)
        {
            if (!networkConnection.IsOwners())
                return;

            var data = dataFunc?.Invoke();

            if (data == sdk.PlayerData.MetaverseData.GetString(metaverseFieldName))
                return;

            sdk.PlayerData.MetaverseData.SetString(metaverseFieldName, data);
            sdk.PlayerData.Save();
        }

        private void TryRemovingServiceFromClientsDataProviderService(NetworkConnection networkConnection)
        {
            if (networkConnection.IsOwners())
                _clientsDataProviderService.ClearClientsData();
            else
                _clientsDataProviderService.TryRemovingClientData(networkConnection);
        }

        private void SetPlayerCustomizationData(SettingClientCustomizationDataBroadcastForClient broadcast,
            Channel channel) =>
            _clientsCustomizationDataProviderService.TrySettingClientCustomizationData(
                broadcast.DataOwnerNetworkConnection,
                broadcast.CustomizationData);

        private void SubscribeShopSettingEquippedItemsModelForChangingClientPlayerCustomizationData(
            ShopSettingEquippedItemsModel shopSettingEquippedItemsModel)
            => shopSettingEquippedItemsModel.ChangedPlayerCustomizationData += customizationData =>
            {
                if (_clientsCustomizationDataProviderService.CanSetClientPlayerCustomizationData(
                        SharedNetworkTools.OwnerConnection, customizationData))
                    _clientManager.Broadcast(new SettingClientCustomizationDataBroadcastForServer(customizationData));
            };

        private void SubscribeShopSettingEquippedItemsModelForChangingOwnerName(
            ShopSettingEquippedItemsModel shopSettingEquippedItemsModel)
            => shopSettingEquippedItemsModel.ChangedNickname += nickname =>
            {
                if (_clientsNicknamesProviderService.CanSetClientNickname(SharedNetworkTools.OwnerConnection,
                        nickname))
                    _clientManager.Broadcast(new SettingNicknameBroadcastForServer(nickname));
            };

        private void InitializeClientServices(ClientServicesInitializationBroadcast broadcast)
        {
            var clientData = broadcast.SerializedClientData.GetDeserializedWithoutNulls<ClientData>();
            _clientsDataProviderService.AddClientData(broadcast.DataOwnerNetworkConnection, clientData);
        }

        private void InitializeClientServices(ClientServicesInitializationBroadcast broadcast, Channel channel)
            => InitializeClientServices(broadcast);

        private void InitializeClientsServices(ClientsServicesInitializationBroadcast broadcast, Channel channel) =>
            broadcast.ClientsServicesInitializationBroadcasts.ForEach(InitializeClientServices);

        private void DisplayMessageOverPlayer(NetworkConnection networkConnection, ChatZone chatZone,
            ChatMessageData chatMessageData) =>
            _playersFactory.ClientsComponents.GetValueOrDefault(networkConnection)?.ChatTextController
                .StartOrContinueShowingText(chatMessageData.Content);

        private async void InitializeHubAndEnterHubState(HubInitializationDependencies dependencies, Channel channel)
        {
            await _hubFactory.GetInitializedHubAndShopAsync(dependencies.HubInstance, _sdk.PlatformNavigation);
            _eventBus.Fire(new EnterClientStateEvent(GameState.Hub));
        }

        private void InitializePlayers(PlayersInitializationBroadcast broadcast, Channel channel)
            => broadcast.Dependencies.ForEach(dependency => InitializePlayer(dependency, channel));

        private async void InitializePlayer(PlayerInitializationDependencies dependencies, Channel channel)
        {
            if (dependencies.Connection.IsOwners())
                InitializeOwnerPlayer(dependencies, channel);
            else
                await _playersFactory.GetInitializedClientPlayerComponents(dependencies.Connection,
                    dependencies.Player.GetComponent<SharedPlayerSerializableComponents>());
        }

        private async void InitializeOwnerPlayer(PlayerInitializationDependencies dependencies, Channel channel)
        {
            await _playersFactory.GetInitializedOwnerPlayerComponentsAsync(dependencies
                .Player.GetComponent<SharedPlayerSerializableComponents>());
        }
    }
}